using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  [Flags]
  public enum CellProperty : int
  {
    None = 0,
    RoomCorner = 1 << 0,
    Left = 1 << 1,
    Right = 1 << 2,
    Top = 1 << 3,
    Bottom = 1 << 4,
  };
}
