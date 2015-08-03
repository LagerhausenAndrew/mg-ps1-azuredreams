using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Index : IDungeonItem, IEquatable<Index>
  {
    public int Column { get; private set; }
    public int Row { get; private set; }

    public Index(int row, int column)
    {
      Column = column;
      Row = row;
    }

    public override int GetHashCode()
    {
      unchecked
      {
        int hash = 17;
        hash = hash * 23 + Column.GetHashCode();
        hash = hash * 23 + Row.GetHashCode();
        return hash;
      }
    }

    public override bool Equals(object obj)
    {
      Index other = obj as Index;
      if (other == null) return false;
      return Equals(other);
    }

    public bool Equals(Index other)
    {
      return other.Row.Equals(Row) && other.Column.Equals(Column);
    }

    public override string ToString()
    {
      return string.Format("{{ r={0},c={1} }}", Row, Column);
    }
  }
}
