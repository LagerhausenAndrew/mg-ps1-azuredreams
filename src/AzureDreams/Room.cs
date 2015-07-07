using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Room
  {
    private readonly Cell[] mCells;

    private readonly RoomBounds mBounds;
    public RoomBounds Bounds { get { return mBounds; } }

    public Room(Cell[] cells, RoomBounds bounds)
    {
      mCells = cells;
      mBounds = bounds;
    }
  }
}
