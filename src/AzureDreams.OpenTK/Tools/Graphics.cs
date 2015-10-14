using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace AzureDreams.OpenGL
{
  public static class Graphics
  {
    private class AlphaBlendSettings : IDisposable
    {
      private bool enabled = false;

      public AlphaBlendSettings(Color4 color)
      {
        if (color.A < 255)
        {
          enabled = true;

          GL.Enable(EnableCap.Blend);
          GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }
      }

      public void Dispose()
      {
        if (enabled)
        {
          GL.Disable(EnableCap.Blend);
        }
      }
    }

    private static IDisposable AlphaBlend(Color4 color)
    {
      return new AlphaBlendSettings(color);
    }

    public static void Begin(float width, float height)
    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Ortho(0, width, height, 0, 0, 1);

      GL.MatrixMode(MatrixMode.Modelview);
      GL.PushMatrix();
      GL.LoadIdentity();
    }

    public static void End()
    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.PopMatrix();
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PopMatrix();
    }

    public static void FillRectangle(Color4 color, float x, float y, float width, float height)
    {
      using (AlphaBlend(color))
      {
        GL.Begin(PrimitiveType.Quads);
        GL.Color4(color);
        GL.Vertex2(x, y);
        GL.Vertex2(x + width, y);
        GL.Vertex2(x + width, y + height);
        GL.Vertex2(x, y + height);
        GL.End();
      }
    }
  }
}
