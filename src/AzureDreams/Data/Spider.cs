using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureDreams
{
  public class Spider : IDungeonItem
  {
    public int Row { get; set; }
    public int Column { get; set; }
    public Direction Direction { get; set; }
    public Direction StartDirection { get; set; }
    public double Moves { get; set; }
    public bool Dead { get; set; }
    public bool Zombie { get; set; }

    public int Key
    {
      get { return Cell.GetHashCode(Row, Column); }
    }
  }
}
