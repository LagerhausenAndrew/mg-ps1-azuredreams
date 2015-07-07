using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  internal static class InternalExtensions
  {
    public static Direction Opposite(this Direction direction)
    {
      switch (direction)
      {
        case Direction.East: return Direction.West;
        case Direction.North: return Direction.South;
        case Direction.South: return Direction.North;
        case Direction.West: return Direction.East;
      }
      throw new InvalidOperationException();
    }

    public static T GetDefault<T>(this IEnumerable<T> sequence)
    {
      return default(T);
    }

    public static T PopAt<T>(this IList<T> list, int index)
    {
      T value = list[index];
      list.RemoveAt(index);
      return value;
    }
  }
}
