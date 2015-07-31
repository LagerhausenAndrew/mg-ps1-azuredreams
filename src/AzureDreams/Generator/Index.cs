using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Index : IDungeonItem
  {
    public int Column { get; private set; }
    public int Row { get; private set; }

    public Index(int row, int column)
    {
      Column = column;
      Row = row;
    }
  }
}
