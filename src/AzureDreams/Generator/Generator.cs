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

    const int MinRoomDistance = 9;
    const int MaxRoomDistance = 99;

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
        while (moved.Indices.Any(dungeon.Exists));

        // we know all the cells are free!
        rooms.Add(moved);
        SpawnRoom(moved);
      }
    }

    private RoomBounds MoveAwayFrom(RoomBounds bounds)
    {
      int horzMod, horzDist;
      int vertMod, vertDist;

      horzMod = Percent(0.5) ? -1 : 1;
      horzDist = random.Next(MinRoomDistance, MaxRoomDistance + 1) * horzMod;

      vertMod = Percent(0.5) ? -1 : 1;
      vertDist = random.Next(MinRoomDistance, MaxRoomDistance + 1) * vertMod;

      var moved = new RoomBounds();
      moved.Bottom = bounds.Bottom;
      moved.Left = bounds.Left;
      moved.Right = bounds.Right;
      moved.Top = bounds.Top;

      int type = random.Next() % 3;
      if (type == 0 || (type == 1 || Percent(0.3)))
      {
        moved.Bottom += vertDist;
        moved.Top += vertDist;
      }
      if (type == 0 || (type == 2 || Percent(0.3)))
      {
        moved.Right += horzDist;
        moved.Left += horzDist;
      }

      return moved;
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
