using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureDreams
{
  public class Spider : IDungeonItem
  {
    public Direction Direction;
    public bool Kill = false;

    public double ItersWithoutTurning = 0;
    public double ItersWithoutCreatingRoom = 0;

    public int Row { get; set; }
    public int Column { get; set; }
  }
}
