using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public struct RoomBounds
  {
    public int Left, Top, Right, Bottom;

    public bool IntersectsWith(RoomBounds box)
    {
      return 
        (box.Left < Right) && 
        (Left < box.Right) && 
        (box.Top < Bottom) && 
        (Top < box.Bottom);
    }

    public RoomBounds Inflate(int xy)
    {
      return Inflate(xy, xy);
    }

    public RoomBounds Inflate(int x, int y)
    {
      return new RoomBounds
      {
        Left = Left - x,
        Top = Top - y,
        Right = Right + x,
        Bottom = Bottom + y,
      };
    }
  }
}
