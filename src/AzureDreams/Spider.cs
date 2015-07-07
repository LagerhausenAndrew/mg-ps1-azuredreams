using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Spider
  {
    public int LastRow { get; private set; }
    public int LastColumn { get; private set; }

    public int Row { get; set; }
    public int Column { get; set; }
    public Direction Direction { get; set; }
    public SpiderState State { get; set; }
    public bool Pass { get; set; }
    public Direction Origin { get; set; }
    public int Moves { get; set; }
    public bool Dead { get; set; }

    public void SavePosition()
    {
      LastRow = Row;
      LastColumn = Column;
    }
  }
}
