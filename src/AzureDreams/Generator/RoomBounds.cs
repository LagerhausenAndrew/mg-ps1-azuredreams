using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureDreams
{
  public class RoomBounds
  {
    public int Left, Top, Right, Bottom;

    public bool Contains(Index index)
    {
      if (index.Row > Bottom) return false;
      if (index.Row < Top) return false;
      if (index.Column > Right) return false;
      if (index.Column < Left) return false;
      return true;
    }

    public IEnumerable<Index> Indices
    {
      get
      {
        int left = Left;
        int top = Top;
        int right = Right;
        int bottom = Bottom;

        int r, c;
        for (r = top; r <= bottom; ++r)
        {
          for (c = left; c <= right; ++c)
          {
            yield return new Index(r, c);
          }
        }
      }
    }

    public RoomBounds Move(int dx, int dy)
    {
      return new RoomBounds
      {
        Bottom = Bottom + dy,
        Left = Left + dx,
        Right = Right + dx,
        Top = Top + dy,
      };
    }

    public RoomBounds Inflate(int dx, int dy)
    {
      return new RoomBounds
      {
        Bottom = Bottom + dy,
        Left = Left - dx,
        Right = Right + dx,
        Top = Top - dy,
      };
    }

    public bool IntersectsWith(RoomBounds other)
    {
      return (other.Left < Right) && (Left < other.Right) && (other.Top < Bottom) && (Top < other.Bottom);
    }
  }
}
