using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public interface IDungeonItem
  {
    int Column { get; }
    int Row { get; }
  }
}
