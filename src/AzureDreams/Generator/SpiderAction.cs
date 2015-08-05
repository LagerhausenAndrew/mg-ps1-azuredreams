using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams
{
  public class SpiderAction : IComparable<SpiderAction>
  {
    public readonly SpiderActionType Type;
    public readonly double Weight;

    public SpiderAction(SpiderActionType type, double weight)
    {
      Type = type;
      Weight = weight;
    }

    public int CompareTo(SpiderAction other)
    {
      return Weight.CompareTo(other.Weight);
    }
  }
}
