using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class Pathfinder
  {
    private class Node : IComparable<Node>
    {
      public double f, g;
      public bool visited;
      public Node parent;

      public readonly Index Index;
      public int Row { get { return Index.Row; } }
      public int Column { get { return Index.Column; } }

      public Node(Index index)
      {
        Index = index;
        Reset();
      }

      public void Reset()
      {
        visited = false;
        f = double.PositiveInfinity;
        g = double.PositiveInfinity;
        parent = null;
      }

      int IComparable<Node>.CompareTo(Node other)
      {
        return f.CompareTo(other.f);
      }
    }

    private readonly Dictionary<Index, Node> environment;
    private readonly Dungeon dungeon;
    private readonly RoomBounds bounds;
    private bool resetNeeded = false;

    public Pathfinder(Dungeon dungeon, RoomBounds bounds)
    {
      this.environment = bounds.Indices.ToDictionary(k => k, v => new Node(v));
      this.dungeon = dungeon;
      this.bounds = bounds;
    }

    public Stack<Index> Calculate(Index start, Index goal)
    {
      Reset();

      var startNode = environment[start];
      var goalNode = environment[goal];
      
      var openset = new List<Node> { startNode };
      startNode.g = 0;
      startNode.f = startNode.g + cost(startNode, goalNode);

      while (openset.Count > 0)
      {
        var current = openset.PopAt(0);
        if (current == goalNode)
        {
          return reconstruct(current);
        }

        current.visited = true;
        foreach (var neighbor in GetNeighbors(current))
        {
          if (!neighbor.visited)
          {
            var g = current.g + cost(current, neighbor);
            var contains = openset.Contains(neighbor);
            if (!contains || g < neighbor.g)
            {
              neighbor.parent = current;
              neighbor.g = g;
              neighbor.f = g + cost(neighbor, goalNode);
              if (!contains)
              {
                Insert(neighbor, openset);
              }
            }
          }
        }
      }

      return null;
    }

    private void Insert(Node node, List<Node> nodes)
    {
      int index = nodes.BinarySearch(node);
      if (index < 0)
      {
        index = ~index;
      }
      nodes.Insert(index, node);
    }

    private IEnumerable<Node> GetNeighbors(Node node)
    {
      int[][] dirs = new int[4][]
      {
        new int[]{ 1, 0 },
        new int[]{ -1, 0 },
        new int[]{ 0, 1 },
        new int[]{ 0, -1 },
      };

      foreach (var dir in dirs)
      {
        var index = new Index(node.Row + dir[0], node.Column + dir[1]);
        Node neighbor;
        if (environment.TryGetValue(index, out neighbor))
        {
          Cell cell;
          if (!dungeon.TryGetCell(index, out cell) || cell.Type == CellType.Floor)
          {
            yield return neighbor;
          }
        }
      }
    }

    private Stack<Index> reconstruct(Node current)
    {
      var stack = new Stack<Index>();
      do
      {
        stack.Push(current.Index);
        current = current.parent;
      }
      while (current != null);
      return stack;
    }

    private double cost(Node a, Node b)
    {
      double dr, dc;
      dr = (b.Row - a.Row);
      dc = (b.Column - a.Column);
      return (dr * dr) + (dc * dc);
    }

    private void Reset()
    {
      if (!resetNeeded)
      {
        return;
      }

      foreach (var kvp in environment)
      {
        kvp.Value.Reset();
      }

      resetNeeded = true;
    }
  }
}
