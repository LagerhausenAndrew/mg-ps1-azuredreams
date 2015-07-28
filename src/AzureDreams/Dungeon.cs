using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Dungeon
  {
    private readonly Dictionary<Tuple<int, int>, Cell> cells = new Dictionary<Tuple<int, int>, Cell>();

    public Cell this[int row, int column]
    {
      get { return cells[Tuple.Create(row, column)]; }
      set { cells[Tuple.Create(row, column)] = value; }
    }

    public Cell this[IDungeonItem item]
    {
      get { return this[item.Row, item.Column]; }
      set { this[item.Row, item.Column] = value; }
    }

    public IEnumerable<Cell> Cells
    {
      get
      {
        return cells.Select(kvp => kvp.Value);
      }
    }

    public Dungeon()
    {
      cells = new Dictionary<Tuple<int, int>, Cell>();
    }

    public bool TryGetValue(IDungeonItem item, out Cell cell)
    {
      return TryGetValue(item.Row, item.Column, out cell);
    }

    public bool TryGetValue(int row, int column, out Cell cell)
    {
      var key = Tuple.Create(row, column);
      return cells.TryGetValue(key, out cell);
    }
  }
}
