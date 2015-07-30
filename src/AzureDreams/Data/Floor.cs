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
    const double SpiderZombieTurnPC = 0.75;
    const double SpiderMovesLen = 12.0;
    const double MaxSpiderLifespan = 256;

    const int RoomAttemtps = 8;

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
      // reset the dungeon
      dungeon.Clear();

      // create a starting room with random bounds
      var rooms = new List<RoomBounds>();
      var start = CreateRoom();
      rooms.Add(start);

      // add spiders for the room
      var spiders = new List<Spider>();
      SpawnSpiders(start, rooms, spiders);

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
          }
          else
          {
            // now that the spider has moved, we need to get the cell that is
            // occupied by this space. 
            // * If the cell is a room wall, then we need to knock down the wall, and kill the spider. We do not spawn more spiders in this case
            // * If the cell is a regular cell, then we need to kill the spider. We do not spawn any more spiders in this case.
            // * If the cell is not occupied, we need to turn (rarely), do nothing, or create a room (based on number of moves)

            Cell cell;
            if (!dungeon.TryGetValue(spider, out cell))
            {
              // if this is a zombie spider, then reset the moves
              if (spider.Zombie)
              {
                spider.Moves = 0;
              }

              // we need to decide on our next course of action. The first action we consider is creating a room.
              // We will create room based on the number of moves we've had
              double pc = (spider.Moves / SpiderMovesLen) * random.NextDouble();
              if (pc > 0.75)
              {
                DoGenerateRoom(spider, spiders, rooms);
                if (!spider.Dead)
                {
                  spider.Zombie = (rooms.Count == roomCount);
                  SpawnFloor(spider);
                }
              }
              else
              {
                SpawnFloor(spider);

                // create a variable to hold the turn percentage
                pc = spider.Zombie
                  ? SpiderZombieTurnPC
                  : SpiderTurnPC;

                // we decided not to create a room. There is a x% chance that we change directions
                if (random.NextDouble() < pc)
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
        SpawnSpiders(room, rooms, spiders);

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

    private void SpawnSpiders(RoomBounds room, List<RoomBounds> rooms, List<Spider> spiders)
    {
      int count = rooms.Count;
      if (count >= roomCount)
      {
        return;
      }

      var shuffled = Enum
        .GetValues(typeof(Direction))
        .Cast<Direction>()
        .OrderBy(d => random.Next());

      double pc = SpiderSpawnPC;
      foreach (var direction in shuffled)
      {
        spiders.Add(CreateSpider(room, direction));
        ++count;
        if (count >= roomCount)
        {
          break;
        }

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
          Left = 0,
          Top = 0,
          Bottom = RandRows(),
          Right = RandColumns(),
        };
      }
      else
      {
        bounds = GenerateRandomBounds(spider);
      }

      if (bounds != null)
      {
        SpawnRoom(bounds, spider);
      }

      return bounds;
    }

    private void SpawnRoom(RoomBounds bounds, Spider spider)
    {
      int r, c, endRow, endColumn, dr, dc;
      endRow = bounds.Bottom;
      endColumn = bounds.Right;

      dr = Math.Sign(endRow - bounds.Top);
      dc = Math.Sign(endColumn - bounds.Left);

      for (r = bounds.Top; !r.CompareTo(endRow).Equals(dr); r += dr)
      {
        bool vertWall = (r == bounds.Top || r == endRow);
        for (c = bounds.Left; !c.CompareTo(endColumn).Equals(dc); c += dc)
        {
          bool horzWall = (c == bounds.Left || c == endColumn);
          Cell cell = new Cell(r, c);
          cell.Type = CellType.Room;
          if (horzWall)
          {
            cell.Type = CellType.Wall;
            cell.IsCorner = vertWall;
          }
          else if (vertWall)
          {
            cell.Type = CellType.Wall;
            cell.IsCorner = horzWall;
          }
          dungeon[cell] = cell;
        }
      }

      if (spider != null)
      {
        dungeon[spider].Type = CellType.Door;
      }
    }

    private RoomBounds GenerateRandomBounds(Spider spider)
    {
      int rows, columns,
        left, right, top, bottom,
        dx, dy, y, x;

      for (int i = 0; i < RoomAttemtps; ++i)
      {
        // get the dimensions of the room
        columns = RandColumns();
        rows = RandRows();
        CalculateBounds(spider, rows, columns, out left, out right, out top, out bottom);

        dx = Math.Sign(right - left);
        dy = Math.Sign(bottom - top);

        bool all = true;
        for (y = top; all && !y.CompareTo(bottom).Equals(dy); y += dy)
        {
          for (x = left; all && !x.CompareTo(right).Equals(dx); x += dx)
          {
            all &= !dungeon.Exists(y, x);
          }
        }

        if (all)
        {
          return new RoomBounds
          {
            Bottom = bottom,
            Left = left,
            Right = right,
            Top = top,
          };
        }
      }

      return null;
    }

    private void CalculateBounds(Spider spider, int rows, int columns, out int left, out int right, out int top, out int bottom)
    {
      left = right = top = bottom = 0;

      var direction = spider.Direction;
      switch (direction)
      {
        case Direction.West:
        case Direction.East:
          {
            if (direction == Direction.East)
            {
              // we're on the west wall of the room
              left = spider.Column;
              right = left + (columns * 2);
            }
            else
            {
              // we're on the easy wall of the room
              right = spider.Column;
              left = right - (columns * 2);
            }

            // no matter what, we're always on the same vertical
            top = spider.Row - rows;
            bottom = top + (rows * 2);
            break;
          }
        case Direction.North:
        case Direction.South:
          {
            if (direction == Direction.South)
            {
              // we're on the north wall of the room
              top = spider.Row;
              bottom = top + (rows * 2);
            }
            else
            {
              // we're on the south wall of the room
              bottom = spider.Row;
              top = bottom - (rows * 2);
            }

            // no matter what, we're always on the same horizontal
            left = spider.Column - columns;
            right = left + (columns * 2);
            break;
          }
      }
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
