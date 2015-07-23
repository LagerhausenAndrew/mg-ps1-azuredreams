using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Floor
  {
    static readonly Direction[] sDirections;
    static Floor()
    {
      sDirections = (Direction[])Enum.GetValues(typeof(Direction));
    }

    private int mRows;
    private int mColumns;
    private int mDesiredRoomCount;

    private readonly Random random;
    private readonly Dictionary<int, Cell> dungeon = new Dictionary<int, Cell>();
    private readonly List<Room> rooms = new List<Room>();
    private readonly List<Spider> spiders = new List<Spider>();

    public IEnumerable<Cell> Cells
    {
      get
      {
        foreach (var kvp in dungeon)
          yield return kvp.Value;
      }
    }

    public Floor(int rows, int columns, int preferredRoomCount = 10)
    {
      mRows = rows;
      mColumns = columns;
      mDesiredRoomCount = preferredRoomCount;

      random = new Random(Environment.TickCount);
      Generate();
    }

    private void Generate()
    {
      Init();
      while (RunOnce()) ;
    }

    private void Init()
    {
      // setup variables for spider creation
      var spiderCount = 1;
      var directions = sDirections.ToList();

      // create the spiders
      for (int i = 0; i < spiderCount; ++i)
      {
        var dir = directions.PopAt(random.Next(directions.Count));
        spiders.Add(new Spider
        {
          Row = random.Next(mRows),
          Column = random.Next(mColumns),
          Direction = dir,
          State = SpiderState.CreateRoom,
          Origin = dir,
          Pass = true,
        });
      }
    }

    private bool RunOnce()
    {
      var spawned = new List<Spider>();

      for (int i = 0; i < spiders.Count; ++i)
      {
        var spider = spiders[i];
        switch (spider.State)
        {
          case SpiderState.CreateRoom:
            {
              spawned.AddRange(CreateRoom(spider));
              break;
            }
          case SpiderState.Move:
            {
              MoveSpider(spider);
              break;
            }
        }
      }

      spiders.RemoveAll(s => s.Dead);
      spiders.AddRange(spawned);
      return spiders.Count > 0;
    }

    private void MoveSpider(Spider spider)
    {
      // update the number of moves made
      spider.Moves++;
      if (spider.Moves > 5 && random.Next(100) <= 5)
      {
        // generate a room!
        spider.State = SpiderState.CreateRoom;
        return;
      }

      // retrieve the key for the spider's position
      int spiderKey = Cell.GetHashCode(spider.Row, spider.Column);

      // we're not generating a room, did we just hit a used cell
      Cell existing;
      if (dungeon.TryGetValue(spiderKey, out existing))
      {
        // we just moved to a used cell...we're dead
        spider.Dead = true;
        if (existing.Type == CellType.Wall)
        {
          if ((existing.Properties & CellProperty.RoomCorner) == 0)
          {
            // knock down the wall here!
            existing.Type = CellType.Exit;
            return;
          }
          else
          {
            // reset the position to the last position
            spider.Dead = false;
            spider.Row = spider.LastRow;
            spider.Column = spider.LastColumn;
            spider.Direction = sDirections
              .Where(d => d != spider.Direction && d != spider.Direction.Opposite())
              .OrderBy(a => random.Next())
              .First();
          }
        }
      }
      else
      {
        existing = null;
      }

      // we're not generating a room, AND we landed on an unused cell, so take this cell
      if (existing == null)
      {
        dungeon[spiderKey] = new Cell(spider.Row, spider.Column)
        {
          Type = CellType.Floor,
        };
      }

      // next, decide which way to move
      var directions = sDirections
        .Where(d => d != spider.Direction.Opposite() && d != spider.Origin.Opposite())
        .Select(d => new
        {
          Weight = (d == spider.Direction) ? 100 : 5,
          Direction = d
        })
        .ToList();

      // sum the weights
      var total = directions.Sum(a => a.Weight);

      // choose a weighted direction
      var randomWeight = random.Next(total);
      var selectedDirection = directions.GetDefault();
      foreach (var item in directions)
      {
        if (randomWeight < item.Weight)
        {
          selectedDirection = item;
          break;
        }

        randomWeight -= item.Weight;
      }

      // set the direction
      if (selectedDirection != null)
      {
        spider.Direction = selectedDirection.Direction;
      }

      // move the spider in that direction
      spider.SavePosition();
      switch (spider.Direction)
      {
        case Direction.East: { ++spider.Column; break; }
        case Direction.North: { --spider.Row; break; }
        case Direction.South: { ++spider.Row; break; }
        case Direction.West: { --spider.Column; break; }
      }
    }

    private Spider[] CreateRoom(Spider spider)
    {
      int left = 0, right = 0, top = 0, bottom = 0;

      int spiderKey = Cell.GetHashCode(spider.Row, spider.Column);
      switch (spider.Direction)
      {
        case Direction.East:
        case Direction.West:
          {
            int up = random.Next(2, 11);
            int down = random.Next(2, 11);
            int columns = random.Next(4, 21);

            top = spider.Row - up;
            bottom = spider.Row + down;

            if (spider.Direction == Direction.East)
            {
              left = spider.Column;
              right = spider.Column + columns;
            }
            else
            {
              left = spider.Column - columns;
              right = spider.Column;
            }

            break;
          }
        case Direction.North:
        case Direction.South:
          {
            int back = random.Next(2, 11);
            int forth = random.Next(2, 11);
            int rows = random.Next(4, 21);

            left = spider.Column - back;
            right = spider.Column + forth;

            if (spider.Direction == Direction.North)
            {
              top = spider.Row - rows;
              bottom = spider.Row;
            }
            else
            {
              top = spider.Row;
              bottom = spider.Row + rows;
            }

            break;
          }
      }

      var bounds = new RoomBounds();
      bounds.Bottom = bounds.Right = int.MinValue;
      bounds.Top = bounds.Left = int.MaxValue;

      var intersects = false;
      var roomLst = new List<Cell>();
      int r, c;

      for (r = top; !intersects && r <= bottom; ++r)
      {
        for (c = left; !intersects && c <= right; ++c)
        {
          var k = Cell.GetHashCode(r, c);
          intersects = dungeon.ContainsKey(k);

          // no point in executing more code if we're intersecting
          if (intersects) continue;

          var type = CellType.Room;
          var properties = CellProperty.None;
          if (c == left || c == right || r == top || r == bottom)
          {
            type = CellType.Wall;
            if (
              (c == left && r == top) ||
              (c == right && r == top) ||
              (c == right && r == bottom) ||
              (c == left && r == bottom))
            {
              properties |= CellProperty.RoomCorner;
            }

            if (c == left) { properties |= CellProperty.Left; }
            if (c == right) { properties |= CellProperty.Right; }
            if (r == top) { properties |= CellProperty.Top; }
            if (r == bottom) { properties |= CellProperty.Bottom; }
          }

          roomLst.Add(new Cell(r, c)
          {
            Type = type,
            Properties = properties,
          });

          bounds.Bottom = Math.Max(bounds.Bottom, r);
          bounds.Left = Math.Min(bounds.Left, c);
          bounds.Right = Math.Max(bounds.Right, c);
          bounds.Top = Math.Min(bounds.Top, r);
        }
      }

      if (!intersects)
      {
        // get the bounds of the room we're trying to create, but
        // extend it by n cells
        var inflated = bounds.Inflate(8);

        // see if the bounds intersects with any of the other room bounds
        foreach (var room in rooms)
        {
          if (room.Bounds.IntersectsWith(inflated))
          {
            intersects = true;
            break;
          }
        }
      }

      var retval = new List<Spider>();
      if (!intersects)
      {
        // add the room to the dungeon
        foreach (var item in roomLst)
        {
          dungeon[item.Key] = item;
        }

        // kill the spider
        spider.Dead = true;

        // make this cell an exit
        if (!spider.Pass)
        {
          Cell match;
          if (dungeon.TryGetValue(spiderKey, out match))
          {
            match.Type = CellType.Exit;
          }
        }

        // determine how many more spiders to spawn
        var spawnCount = random.Next(4);

        // retrieve the valid directions
        if (spawnCount > 0 && random.Next(mDesiredRoomCount) < Math.Max(mDesiredRoomCount - rooms.Count, 1))
        {
          var directions = Enum
            .GetValues(typeof(Direction))
            .Cast<Direction>()
            .Where(d => d != spider.Direction.Opposite())
            .ToList();

          // iterate the spawn count
          for (int s = 0; s < spawnCount; ++s)
          {
            // choose a random direction to move in
            var direction = directions.PopAt(random.Next(directions.Count));
            int row = 0, column = 0;
            switch (direction)
            {
              case Direction.East:
              case Direction.West:
                {
                  var tc = direction == Direction.East ? right : left;
                  var dc = direction == Direction.East ? 1 : -1;

                  var wall = roomLst
                    .Where(l => (l.Column == tc) && (l.Row != top && l.Row != bottom))
                    .ToArray();

                  var cell = wall[random.Next(wall.Length)];
                  row = cell.Row;
                  column = cell.Column + dc;
                  cell.Type = CellType.Exit;
                  break;
                }
              case Direction.North:
              case Direction.South:
                {
                  var tr = direction == Direction.North ? top : bottom;
                  var dr = direction == Direction.North ? -1 : 1;

                  var wall = roomLst
                    .Where(l => (l.Row == tr) && (l.Column != left && l.Column != right))
                    .ToArray();

                  var cell = wall[random.Next(wall.Length)];
                  row = cell.Row + dr;
                  column = cell.Column;
                  cell.Type = CellType.Exit;
                  break;
                }
            }

            // add a new spider
            retval.Add(new Spider
            {
              Column = column,
              Dead = false,
              Direction = direction,
              Moves = 0,
              Origin = direction,
              Row = row,
              State = SpiderState.Move,
            });
          }
        }

        // save the position for each spawned spider
        retval.ForEach(s => s.SavePosition());

        // increment the room count
        rooms.Add(new Room(roomLst.ToArray(), bounds));
      }
      else
      {
        // keep moving!
        spider.State = SpiderState.Move;

        // reset the moves so I don't keep trying to create a room!
        spider.Moves = 0;
      }
      return retval.ToArray();
    }
  }
}
