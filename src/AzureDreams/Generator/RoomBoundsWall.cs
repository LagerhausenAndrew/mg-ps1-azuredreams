using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class RoomBoundsWall
  {
    public readonly Direction Wall;
    public readonly Index[] Indices;
    public readonly RoomBounds Parent;

    public RoomBoundsWall(RoomBounds parent, Direction wall, IEnumerable<Index> indices)
    {
      Parent = parent;
      Wall = wall;
      Indices = indices.ToArray();
    }
  }
}
