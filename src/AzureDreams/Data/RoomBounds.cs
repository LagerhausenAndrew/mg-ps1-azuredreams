using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureDreams
{
  class RoomBounds
  {
    public int Row;
    public int Column;
    public int Rows;
    public int Columns;

    public int Left { get { return Column; } }
    public int Right { get { return Column + Columns - 1; } }
    public int Top { get { return Row; } }
    public int Bottom { get { return Row + Rows - 1; } }
  }
}
