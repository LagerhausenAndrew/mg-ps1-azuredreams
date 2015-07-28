using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Floor
  {
    const double SpiderSpawnPC = 1.65;
    const double SpiderTurnPC = 0.10;
    const double SpiderMovesLen = 32.0;
    const double MaxSpiderLifespan = 128;

    const int MaxRoomRows = 16;
    const int MaxRoomColumns = 16;

    const int MinRoomRows = 4;
    const int MinRoomColumns = 4;

    static readonly Dictionary<Direction, Direction> sOpposite;
    static readonly LinkedList<Direction> sLoop = new LinkedList<Direction>();

    static Floor()
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

    public Floor(int roomCount = 10)
    {
      this.roomCount = roomCount;
      random = new Random(Environment.TickCount);
      dungeon = new Dungeon();
    }

    public IEnumerable<bool> Generate()
    {
      // create a starting room with random bounds
      var rooms = new List<RoomBounds>();
      var start = CreateRoom();
      rooms.Add(start);

      // add spiders for the room
      var spiders = new List<Spider>();
      SpawnSpiders(start, spiders);

      // go through and move the spiders
      while (spiders.Count > 0)
      {
        var spiderPool = spiders.ToArray();
        foreach (var spider in spiderPool)
        {
          int row, column;
          row = spider.Row;
          column = spider.Column;

          // move the spider in the direction it is facing
          DoMoveSpider(spider);

          // if this spider has moved too much, then kill it
          if (spider.Moves >= MaxSpiderLifespan)
          {
            spider.Dead = true;
            continue;
          }

          // now that the spider has moved, we need to get the cell that is
          // occupied by this space. 
          // * If the cell is a room wall, then we need to knock down the wall, and kill the spider. We do not spawn more spiders in this case
          // * If the cell is a regular cell, then we need to kill the spider. We do not spawn any more spiders in this case.
          // * If the cell is not occupied, we need to turn (rarely), do nothing, or create a room (based on number of moves)

          Cell cell;
          if (!dungeon.TryGetValue(spider, out cell))
          {
            // we need to decide on our next course of action. The first action we consider is creating a room.
            // We will create room based on the number of moves we've had
            double pc = spider.Moves / SpiderMovesLen;
            if (random.NextDouble() < pc)
            {
              DoGenerateRoom(spider, spiders, rooms);
              if (!spider.Dead)
              {
                SpawnFloor(spider);
              }
            }
            else
            {
              SpawnFloor(spider);

              // we decided not to create a room. There is a x% chance that we change directions
              if (random.NextDouble() < SpiderTurnPC)
              {
                DoTurnSpider(spider);
              }
            }
          }
          else
          {
            // this is not a free cell. If it's a wall, then attempt to knock it down
            if (cell.Type == CellType.Wall && !cell.IsCorner)
            {
              cell.Type = CellType.Door;
            }
            spider.Dead = true;
          }
        }

        // remove all the dead spiders
        spiders.RemoveAll(s => s.Dead);
        yield return true;
      }
    }

    private void SpawnFloor(Spider spider)
    {
      // this is a free cell! We need to create a cell out of this
      var cell = new Cell(spider.Row, spider.Column);
      cell.Type = CellType.Floor;
      dungeon[spider] = cell;
    }

    private void DoTurnSpider(Spider spider)
    {
      var node = sLoop.Find(spider.Direction);
      var turned = (random.NextDouble() < 0.5)
        ? node.Next()
        : node.Previous();
      spider.Direction = turned.Value;
    }

    private void DoGenerateRoom(Spider spider, List<Spider> spiders, List<RoomBounds> rooms)
    {
      if (rooms.Count < roomCount)
      {
        // attempt to create a room
        var room = CreateRoom(spider);
        if (room == null) return;

        // add the room to the list
        rooms.Add(room);

        // spawn spiders for the room
        SpawnSpiders(room, spiders);

        // kill this spider
        spider.Dead = true;
      }
    }

    private void DoMoveSpider(Spider spider)
    {
      switch (spider.Direction)
      {
        case Direction.East: spider.Column++; break;
        case Direction.North: spider.Row--; break;
        case Direction.South: spider.Row++; break;
        case Direction.West: spider.Column--; break;
      }

      spider.Moves++;
    }

    private void SpawnSpiders(RoomBounds room, List<Spider> spiders)
    {
      var shuffled = Enum
        .GetValues(typeof(Direction))
        .Cast<Direction>()
        .OrderBy(d => random.Next());

      double pc = SpiderSpawnPC;
      foreach (var direction in shuffled)
      {
        spiders.Add(CreateSpider(room, direction));

        pc = pc / 2;
        if (pc <= random.NextDouble())
        {
          break;
        }
      }
    }

    private Spider CreateSpider(RoomBounds room, Direction direction)
    {
      var spider = new Spider();
      spider.Direction = direction;
      spider.StartDirection = direction;
      spider.Moves = 0;
      PlaceSpiderOnWall(room, spider);
      return spider;
    }

    private void PlaceSpiderOnWall(RoomBounds room, Spider spider)
    {
      int minC = 0, maxC = 0;
      int minR = 0, maxR = 0;

      switch (spider.Direction)
      {
        case Direction.West:
        case Direction.East:
          {
            // slide up and down the bottom
            minR = room.Top;
            maxR = room.Bottom;

            // stick to the horizontal side
            minC = maxC = (spider.Direction == Direction.East)
              ? room.Right
              : room.Left;
            break;
          }
        case Direction.North:
        case Direction.South:
          {
            // slide left and right
            minC = room.Left;
            maxC = room.Right;

            // stick to the vertical side
            minR = maxR = (spider.Direction == Direction.South)
              ? room.Bottom
              : room.Top;
            break;
          }
      }

      spider.Column = (minC == maxC)
        ? minC
        : random.Next(minC + 1, maxC);

      spider.Row = (minR == maxR)
        ? minR
        : random.Next(minR + 1, maxR);

      dungeon[spider].Type = CellType.Door;
    }

    private RoomBounds CreateRoom(Spider spider = null)
    {
      RoomBounds bounds = null;

      if (spider == null)
      {
        bounds = new RoomBounds
        {
          Row = 0,
          Column = 0,
          Rows = RandRows(),
          Columns = RandColumns(),
        };
      }
      else
      {
        bounds = GenerateRandomBounds(spider);
      }

      if (bounds.Columns > 1 && bounds.Rows > 1)
      {
        SpawnRoom(bounds, spider);
      }
      else
      {
        bounds = null;
      }

      return bounds;
    }

    private void SpawnRoom(RoomBounds bounds, Spider spider)
    {
      int r, c, rows, columns;
      for (r = bounds.Row, rows = 0; rows < bounds.Rows; ++r, ++rows)
      {
        for (c = bounds.Column, columns = 0; columns < bounds.Columns; ++c, ++columns)
        {
          Cell cell;
          if (!dungeon.TryGetValue(r, c, out cell))
          {
            cell = new Cell(r, c);
            dungeon[r, c] = cell;
          }

          if (c == bounds.Column || columns == bounds.Columns - 1)
          {
            // we're on the left or right edge
            cell.Type = CellType.Wall;
            cell.IsCorner = (r == bounds.Row || rows == bounds.Rows - 1);
          }
          else if (r == bounds.Row || rows == bounds.Rows - 1)
          {
            // we're on the top or bottom edge
            cell.Type = CellType.Wall;
            cell.IsCorner = (c == bounds.Column || columns == bounds.Columns - 1);
          }
          else
          {
            cell.Type = CellType.Room;
            cell.IsCorner = false;
          }
        }
      }

      if (spider != null)
      {
        dungeon[spider].Type = CellType.Door;
      }
    }

    private RoomBounds GenerateRandomBounds(Spider spider)
    {
      RoomBounds bounds = new RoomBounds();

      int r, c, rows, columns;
      for (int i = 0; i < 5; ++i)
      {
        columns = RandColumns();
        rows = RandRows();

        switch (spider.Direction)
        {
          case Direction.East:
            {
              bounds.Row = spider.Row - rows;
              bounds.Column = spider.Column;
              break;
            }
          case Direction.South:
            {
              bounds.Row = spider.Row;
              bounds.Column = spider.Column - columns;
              break;
            }
          case Direction.West:
          case Direction.North:
            {
              bounds.Row = spider.Row - rows;
              bounds.Column = spider.Column - columns;
              break;
            }
        }

        columns *= 2;
        rows *= 2;

        bool all = true;
        for (r = 0; all && r < rows; ++r)
        {
          for (c = 0; all && c < columns; ++c)
          {
            Cell cell;
            all &= !dungeon.TryGetValue(r + bounds.Row, c + bounds.Column, out cell);
          }
        }

        if (all)
        {
          bounds.Rows = rows;
          bounds.Columns = columns;
          break;
        }
      }

      return bounds;
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
