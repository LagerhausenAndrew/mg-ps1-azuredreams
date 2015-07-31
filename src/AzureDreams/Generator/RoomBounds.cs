using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AzureDreams
{
  public class RoomBounds
  {
    public int Left, Top, Right, Bottom;

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
  }
}
