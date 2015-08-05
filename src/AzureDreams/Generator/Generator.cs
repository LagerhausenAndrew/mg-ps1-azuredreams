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

      // create a list to hold the spiders and rooms
      var spiders = new List<Spider>();
      var rooms = new List<RoomBounds>();

      // create a room at 0,0 with a random size
      var bounds = CreateBoundsFromCenter(0, 0, RandColumns(), RandRows());
      rooms.Add(bounds);
      SpawnRoom(bounds);

      // create a variable to hold the available room count
      int availableRoomCount = (roomCount - rooms.Count);

      // spawn x number of spiders, and have each one take up a room slot
      SpawnSpidersForRoom(bounds, spiders, ref availableRoomCount);

      // as long as we have spiders, then keep going
      while (spiders.Count > 0)
      {
        for (int i = spiders.Count - 1; i > -1; --i)
        {
          // get the spider at this point
          var spider = spiders[i];

          // move the spider in the direction it's supposed to go
          MoveSpider(spider);

          // create a list to hold the available actions
          Cell cell;
          var actions = GenerateDecisions(spider, out cell);
          if (spider.Kill)
          {
            // Since the spider did not generate a room, let's give back the room slot
            ++availableRoomCount;
            spiders.RemoveAt(i);
            continue;
          }

          // now, we need to choose an action
          var action = ChooseAction(actions);
          switch (action.Type)
          {
            case SpiderActionType.GenerateRoom:
              {
                // no matter what, reset the iterations
                spider.ItersWithoutCreatingRoom = 0;

                // inflate the existing rooms
                var inflated = rooms.Select(r => r.Inflate(MinRoomDistance, MinRoomDistance)).ToArray();

                // now, attempt to a create a room. If we fail to create a room, then
                // this spider goes back to doing something else until its dead
                RoomBounds room = null;
                bool validRoom = false;
                for (int attempts = 10; !validRoom && attempts > 0; --attempts)
                {
                  room = CreateBoundsFromWall(spider);
                  validRoom = !inflated.Any(r => r.IntersectsWith(room));
                }

                if (validRoom)
                {
                  // the spider is dead, so remove it
                  spiders.RemoveAt(i);

                  // spawn a room from the bounds
                  rooms.Add(room);
                  SpawnRoom(room);

                  // create a door at the spiders current location
                  dungeon[spider].Type = CellType.Door;

                  // spawn spiders for the room excluding the direction we came in
                  SpawnSpidersForRoom(room, spiders, ref availableRoomCount, sOpposite[spider.Direction]);
                }

                break;
              }
            case SpiderActionType.Nothing:
              {
                // generate a floor if the cell is null
                if (cell == null)
                {
                  var floor = new Cell(spider.Row, spider.Column);
                  floor.Type = CellType.Floor;
                  dungeon.Add(floor);
                }
                break;
              }
            case SpiderActionType.Turn:
              {
                // turn the spider
                var node = sLoop.Find(spider.Direction);
                if (Percent(0.5))
                {
                  spider.Direction = node.Next();
                }
                else
                {
                  spider.Direction = node.Previous();
                }
                break;
              }
          }
        }
      }
    }

    private SpiderAction ChooseAction(List<SpiderAction> actions)
    {
      // now that we have the actions, let's choose an action
      double rnd = random.NextDouble() * actions.Sum(a => a.Weight);
      return actions.FirstOrDefault(s =>
      {
        if (rnd < s.Weight)
        {
          return true;
        }
        else
        {
          rnd -= s.Weight;
          return false;
        }
      });
    }

    private List<SpiderAction> GenerateDecisions(Spider spider, out Cell cell)
    {
      // create a list of actions
      var actions = new List<SpiderAction>();

      // has the spider moved into a cell it shouldn't be in?
      if (dungeon.TryGetCell(spider, out cell))
      {
        if (cell.Type != CellType.Floor)
        {
          // the spider has moved to an occupied cell. We need to kill it and move on.
          spider.Kill = true;
        }
        else
        {
          // the spider has moved onto a cell which is a floor. We can only turn
          // or move forward from here. Let's heavily favor moving forward over turning,
          // unless we've been moving forward for awhile
          double turn = Math.Min(0.95, CreatePercentage(spider.ItersWithoutTurning));
          actions.Add(new SpiderAction(SpiderActionType.Turn, turn));
          actions.Add(new SpiderAction(SpiderActionType.Nothing, 1.0 - turn));
        }
      }
      else
      {
        // if we get here, then no cell exists. Make sure the cell is null for the
        // part of the algorithm outside this if check
        cell = null;

        // next, we need to determine the weight for our actions
        double room = Math.Min(0.95, CreatePercentage(spider.ItersWithoutCreatingRoom));
        double turn = Math.Min(0.75, CreatePercentage(spider.ItersWithoutTurning));
        actions.Add(new SpiderAction(SpiderActionType.GenerateRoom, room));
        actions.Add(new SpiderAction(SpiderActionType.Turn, turn));
        actions.Add(new SpiderAction(SpiderActionType.Nothing, 1.75 - (room + turn)));
      }

      // sort the actions by weight
      actions.Sort();
      return actions;
    }

    private double CreatePercentage(double value)
    {
      return 0.1 + (value * 0.1);
    }

    private void MoveSpider(Spider spider)
    {
      switch (spider.Direction)
      {
        case Direction.East: spider.Column++; break;
        case Direction.North: spider.Row--; break;
        case Direction.South: spider.Row++; break;
        case Direction.West: spider.Column--; break;
      }
    }

    private void SpawnSpidersForRoom(RoomBounds room, List<Spider> spiders, ref int availableRoomCount, Direction? excluding = null)
    {
      var directions = sOpposite.Keys.Shuffle(random);
      if (excluding != null)
      {
        directions = directions.Except(excluding.Value);
      }

      var pc = 1.0;
      foreach (var direction in directions)
      {
        // don't spawn any spiders if there are no slots available
        if (availableRoomCount <= 0)
        {
          availableRoomCount = 0;
          break;
        }

        // if we have no chance of spawning the spider, then continue
        if (!Percent(pc))
        {
          continue;
        }

        // we're going to spawn a spider which will potentially generate a room. So we need
        // to take a room slot
        --availableRoomCount;

        // now, we need to spawn a spider, and place it on the corresponding wall of the room
        var spider = CreateSpiderOnWall(room, direction);
        spiders.Add(spider);

        // half the probability for next time
        pc = pc / 2.0;
      }
    }

    private Spider CreateSpiderOnWall(RoomBounds room, Direction direction)
    {
      // retrieve a cell for the door based on the direction within the room
      Func<Index, bool> predicate = null;
      switch (direction)
      {
        case Direction.East: predicate = (i => i.Column == room.Right); break;
        case Direction.North: predicate = (i => i.Row == room.Top); break;
        case Direction.South: predicate = (i => i.Row == room.Bottom); break;
        case Direction.West: predicate = (i => i.Column == room.Left); break;
      }

      // get all the cells that match the predicate and aren't a corner. Then, get a random
      // cell from those that match
      var cell = room.Indices
        .Where(predicate)
        .Select(i => dungeon[i])
        .Where(c => !c.IsCorner)
        .Shuffle(random)
        .First();

      // create a spider which is on this cell
      var spider = new Spider();
      spider.Row = cell.Row;
      spider.Column = cell.Column;
      spider.Direction = direction;

      // create a door at this location
      cell.Type = CellType.Door;

      // give the spider back to the caller
      return spider;
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

    private RoomBounds CreateBoundsFromWall(Spider spider)
    {
      RoomBounds bounds = new RoomBounds();
      int columns = RandColumns();
      int rows = RandRows();

      switch (spider.Direction)
      {
        case Direction.West:
        case Direction.East:
          {
            bounds.Top = spider.Row;
            bounds.Bottom = spider.Row + rows;
          }
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
