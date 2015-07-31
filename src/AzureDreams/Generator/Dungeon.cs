using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Dungeon
  {
    static Tuple<int, int> key(int row, int column)
    {
      return Tuple.Create(row, column);
    }

    private readonly Dictionary<Tuple<int, int>, Cell> cells = new Dictionary<Tuple<int, int>, Cell>();

    public Cell this[int row, int column]
    {
      get { return cells[key(row, column)]; }
      set { cells[key(row, column)] = value; }
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

    public void Add(Cell cell)
    {
      cells.Add(key(cell.Row, cell.Column), cell);
    }

    public bool TryGetCell(IDungeonItem item, out Cell cell)
    {
      return TryGetCell(item.Row, item.Column, out cell);
    }

    public bool TryGetCell(int row, int column, out Cell cell)
    {
      return cells.TryGetValue(key(row, column), out cell);
    }

    public void Clear()
    {
      cells.Clear();
    }

    public bool Exists(int row, int column)
    {
      return cells.ContainsKey(key(row, column));
    }

    public bool Exists(IDungeonItem item)
    {
      return Exists(item.Row, item.Column);
    }
  }
}
