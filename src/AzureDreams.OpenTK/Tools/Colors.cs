using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;

namespace AzureDreams.OpenGL
{
  public static class Colors
  {
    public static Color4 Lerp(Color4 c1, Color4 c2, float blend)
    {
      Vector4 v1 = new Vector4(c1.R, c1.G, c1.B, c1.A);
      Vector4 v2 = new Vector4(c2.R, c2.G, c2.B, c2.A);

      Vector4 result;
      Vector4.Lerp(ref v1, ref v2, blend, out result);

      return new Color4(result.X, result.Y, result.Z, result.W);
    }
  }
}
