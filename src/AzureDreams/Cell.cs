using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Cell : IEquatable<Cell>
  {
    public static int GetHashCode(int row, int column)
    {
      unchecked
      {
        return ((row << 16) | column);
      }
    }

    private readonly int mRow;
    public int Row { get { return mRow; } }

    private readonly int mColumn;
    public int Column { get { return mColumn; } }

    private readonly int mKey;
    public int Key { get { return mKey; } }

    public CellType Type { get; set; }
    public CellProperty Properties { get; set; }

    public Cell(int row, int column)
    {
      mRow = row;
      mColumn = column;
      mKey = GetHashCode(mRow, mColumn);
    }

    public override int GetHashCode()
    {
      return mKey;
    }

    public override bool Equals(object obj)
    {
      Cell cell = obj as Cell;
      if (cell == null) return false;
      return Equals(cell);
    }

    public bool Equals(Cell other)
    {
      return (mRow == other.mRow) && (mColumn == other.mColumn);
    }
  }
}
