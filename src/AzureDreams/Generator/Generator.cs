using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Generator
  {
    const int MaxRoomRows = 16;
    const int MaxRoomColumns = 16;

    const int MinRoomRows = 4;
    const int MinRoomColumns = 4;

    const int MinRoomDistance = 4;
    const int MaxRoomDistance = 16;

    static readonly Dictionary<Direction, Direction> sOpposite;
    static readonly LinkedList<Direction> sLoop = new LinkedList<Direction>();
    static readonly Dictionary<RoomWall, int> sWallCounts = new Dictionary<RoomWall, int>();

    static Generator()
    {
      sOpposite = new Dictionary<Direction, Direction>();
      sOpposite[Direction.East] = Direction.West;
      sOpposite[Direction.North] = Direction.South;
      sOpposite[Direction.South] = Direction.North;
      sOpposite[Direction.West] = Direction.East;

      sLoop.AddLast(Direction.North);
      sLoop.AddLast(Direction.East);
      sLoop.AddLast(Direction.South);
      sLoop.AddLast(Direction.West);
    }

    private readonly int roomCount;
    private readonly Random random;
    private readonly Dungeon dungeon;

    public IEnumerable<Cell> Cells
    {
      get { return dungeon.Cells; }
    }

    public Generator(int roomCount = 10)
    {
      this.roomCount = roomCount;
      random = new Random(Environment.TickCount);
      dungeon = new Dungeon();
    }

    public IEnumerable<bool> Generate()
    {
      // reset the dungeon
      dungeon.Clear();

      // then, we need to generate random rooms
      var rooms = new List<RoomBounds>();

      // create a room at 0,0 with a random size
      var bounds = CreateBoundsFromCenter(0, 0, RandColumns(), RandRows());
      rooms.Add(bounds);
      SpawnRoom(bounds);

      // continue to generate rooms while we can
      while (rooms.Count < roomCount)
      {
        yield return true;

        // choose a source room and move away from it
        RoomBounds moved = null;
        do
        {
          var source = rooms[random.Next(rooms.Count)];
          moved = MoveAwayFrom(source);
        }
        while (moved.Indices.Any(dungeon.Exists) || IsTooClose(moved, rooms));

        // we know all the cells are free!
        rooms.Add(moved);
        SpawnRoom(moved);
      }

      // create the path finder
      var pathfinder = new Pathfinder(dungeon, CalculateBoundingBox());

      // create a list of spiders
      var spiders = new List<Spider>();

      // next, we want to connect the rooms. We want to make sure that each room
      // is connected at least once. After that, we switch to probability based
      // connection
      double probability = 0;
      bool alwaysCalculateProbability = false;
      do
      {
        // choose two random rooms and assign a spider to connect their walls
        var pool = rooms.Where(r => r.Walls.Count > 0).ToList();
        if (pool.Count == 1)
        {
          break;
        }

        var room1 = pool.Fetch(random);
        var room2 = pool.Fetch(random);
        var room1Wall = room1.Walls.Fetch(random);
        var room2Wall = room2.Walls.Fetch(random);
        
        // if we weren't able to add spiders, then add back the rooms and walls
        var succeeded = AddSpider(spiders, room1, room1Wall, room2, room2Wall, pathfinder);
        if (!succeeded)
        {
          room1.Walls.Add(room1Wall);
          room2.Walls.Add(room2Wall);
          pool.Add(room1);
          pool.Add(room2);
        }

        // if there are still rooms that haven't been assigned, we must continue this process
        if (!alwaysCalculateProbability && rooms.Any(r => r.Walls.Count == 4))
        {
          probability = 1.0;
        }
        else
        {
          // all the rooms have been assigned at least once. Should we continue?
          double expected = rooms.Count * 4.0;
          double actual = rooms.Sum(r => r.Walls.Count);
          probability = (actual / expected);
          alwaysCalculateProbability = true;
        }
      }
      while (probability > random.NextDouble());

      // go through the spiders
      while (spiders.Count > 0)
      {
        for (int i = spiders.Count - 1; i > -1; --i)
        {
          var spider = spiders[i];
          MoveSpider(spider);
          if (spider.Dead)
          {
            spiders.RemoveAt(i);
          }
        }
        yield return true;
      }
    }

    private bool IsTooClose(RoomBounds room, List<RoomBounds> rooms)
    {
      var movement = new[]
      {
        new { X = -MinRoomDistance, Y = 0 },
        new { X = MinRoomDistance, Y = 0 },
        new { X = 0, Y = -MinRoomDistance },
        new { X = 0, Y = MinRoomDistance },
      };

      return movement.Any(m =>
      {
        var moved = room.Move(m.X, m.Y);
        return rooms.Any(moved.IntersectsWith);
      });
    }

    private RoomBounds CalculateBoundingBox()
    {
      int 
        minR = int.MaxValue, 
        minC = int.MaxValue, 
        maxR = int.MinValue, 
        maxC = int.MinValue;

      foreach (var c in dungeon.Cells)
      {
        minR = Math.Min(minR, c.Row);
        minC = Math.Min(minC, c.Column);
        maxR = Math.Max(maxR, c.Row);
        maxC = Math.Max(maxC, c.Column);
      }

      int inflation = 16;
      return new RoomBounds
      {
        Bottom = maxR + inflation,
        Left = minC - inflation,
        Right = maxC + inflation,
        Top = minR - inflation,
      };
    }

    private void MoveSpider(Spider spider)
    {
      if (spider.Path == null)
      {
        spider.Dead = true;
      }
      else if (spider.Path.Count > 0)
      {
        var next = spider.Path.Pop();
        Cell cell;
        if (!dungeon.TryGetCell(next, out cell))
        {
          cell = new Cell(next.Row, next.Column);
          cell.Type = CellType.Floor;
          dungeon[next] = cell;
        }
      }
      else
      {
        spider.Dead = true;
      }
    }

    private bool AddSpider(List<Spider> spiders, RoomBounds room1, RoomWall room1Wall, RoomBounds room2, RoomWall room2Wall, Pathfinder pathfinder)
    {
      var start = GetIndex(room1, room1Wall);
      var end = GetIndex(room2, room2Wall);

      var spider = new Spider();
      spider.Path = pathfinder.Calculate(start, end);
      if (spider.Path != null)
      {
        dungeon[start].Type = CellType.Door;
        dungeon[end].Type = CellType.Door;
        spiders.Add(spider);
      }

      return (spider.Path != null);
    }

    private Index GetIndex(RoomBounds room, RoomWall wall)
    {
      Index index = null;
      switch (wall)
      {
        case RoomWall.Top:
        case RoomWall.Bottom:
          {
            index = new Index((wall == RoomWall.Top) ? room.Top : room.Bottom,
              random.Next(room.Left + 1, room.Right));
            break;
          }
        case RoomWall.Left:
        case RoomWall.Right:
          {
            index = new Index(random.Next(room.Top + 1, room.Bottom),
              (wall == RoomWall.Left) ? room.Left : room.Right);
            break;
          }
      }
      return index;
    }

    private RoomBounds MoveAwayFrom(RoomBounds bounds)
    {
      int x, y;
      x = bounds.Left;
      y = bounds.Top;

      int width = RandColumns();
      int height = RandRows();

      int dx = Percent(0.5) ? -1 : 1;
      int dy = Percent(0.5) ? -1 : 1;

      if (Percent(0.5))
      {
        x += RandDistance() * dx;
        if (Percent(0.25))
        {
          y += RandDistance() * dy;
        }
      }
      else
      {
        y += RandDistance() * dy;
        if (Percent(0.1))
        {
          x += RandDistance() * dx;
        }
      }

      return new RoomBounds
      {
        Bottom = y + height,
        Left = x,
        Right = x + width,
        Top = y,
      };
    }

    private void SpawnRoom(RoomBounds bounds)
    {
      bool isHorzWall, isVertWall;

      foreach (var index in bounds.Indices)
      {
        var cell = new Cell(index.Row, index.Column);
        cell.Type = CellType.Room;

        isHorzWall = (index.Column == bounds.Left || index.Column == bounds.Right);
        isVertWall = (index.Row == bounds.Top || index.Row == bounds.Bottom);

        if (isHorzWall)
        {
          cell.Type = CellType.Wall;
          cell.IsCorner = isVertWall;
        }
        else if (isVertWall)
        {
          cell.Type = CellType.Wall;
          cell.IsCorner = isHorzWall;
        }

        dungeon.Add(cell);
      }
    }

    private RoomBounds CreateBoundsFromCenter(int centerColumn, int centerRow, int columns, int rows)
    {
      int left = centerColumn - (columns / 2);
      int top = centerRow - (rows / 2);
      int right = left + columns;
      int bottom = top + rows;
      return new RoomBounds
      {
        Bottom = bottom,
        Left = left,
        Right = right,
        Top = top,
      };
    }

    private bool Percent(double pc)
    {
      return random.NextDouble() < pc;
    }

    private int RandDistance()
    {
      return random.Next(MinRoomDistance, MaxRoomDistance + 1);
    }

    private int RandColumns()
    {
      return random.Next(MinRoomColumns, MaxRoomColumns + 1);
    }

    private int RandRows()
    {
      return random.Next(MinRoomRows, MaxRoomRows + 1);
    }
  }
}
