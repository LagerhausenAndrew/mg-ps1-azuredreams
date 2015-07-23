using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureDreams.WinForms
{
  public static class Colors
  {
    private static float lerp(float value1, float value2, float amount)
    {
      return value1 + (value2 - value1) * amount;
    }

    public static Color Lerp(Color val1, Color val2, float amt)
    {
      return Color.FromArgb(
        (int)lerp(val1.A, val2.A, amt),
        (int)lerp(val1.R, val2.R, amt),
        (int)lerp(val1.G, val2.G, amt),
        (int)lerp(val1.B, val2.B, amt));
    }
  }
}
