using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AzureDreams.MonoDirectX
{
  public static class SpriteBatchExtensions
  {
    private struct HLSColor
    {
      private const int ShadowAdj = -333;
      private const int HilightAdj = 500;
      private const int WatermarkAdj = -50;
      private const int Range = 240;
      private const int HLSMax = 240;
      private const int RGBMax = 255;
      private const int Undefined = 160;
      private readonly int hue;
      private readonly int saturation;
      private readonly int luminosity;

      public int Luminosity
      {
        get { return this.luminosity; }
      }

      public HLSColor(Color color)
      {
        int r = (int)color.R;
        int g = (int)color.G;
        int b = (int)color.B;
        int num4 = Math.Max(Math.Max(r, g), b);
        int num5 = Math.Min(Math.Min(r, g), b);
        int num6 = num4 + num5;
        this.luminosity = (num6 * 240 + 255) / 510;
        int num7 = num4 - num5;
        if (num7 == 0)
        {
          this.saturation = 0;
          this.hue = 160;
        }
        else
        {
          if (this.luminosity <= 120)
          {
            this.saturation = (num7 * 240 + num6 / 2) / num6;
          }
          else
          {
            this.saturation = (num7 * 240 + (510 - num6) / 2) / (510 - num6);
          }
          int num8 = ((num4 - r) * 40 + num7 / 2) / num7;
          int num9 = ((num4 - g) * 40 + num7 / 2) / num7;
          int num10 = ((num4 - b) * 40 + num7 / 2) / num7;
          if (r == num4)
          {
            this.hue = num10 - num9;
          }
          else if (g == num4)
          {
            this.hue = 80 + num8 - num10;
          }
          else
          {
            this.hue = 160 + num9 - num8;
          }
          if (this.hue < 0)
          {
            this.hue += 240;
          }
          if (this.hue > 240)
          {
            this.hue -= 240;
          }
        }
      }

      public Color Darker(float percDarker)
      {
        int num4 = 0;
        int num5 = this.NewLuma(-333, true);
        return this.ColorFromHLS(this.hue, num5 - (int)((num5 - num4) * percDarker), this.saturation);
      }

      public override bool Equals(object o)
      {
        bool result;
        if (!(o is HLSColor))
        {
          result = false;
        }
        else
        {
          HLSColor color = (HLSColor)o;
          result = (this.hue == color.hue && this.saturation == color.saturation && this.luminosity == color.luminosity);
        }
        return result;
      }

      public static bool operator ==(HLSColor a, HLSColor b)
      {
        return a.Equals(b);
      }

      public static bool operator !=(HLSColor a, HLSColor b)
      {
        return !a.Equals(b);
      }

      public override int GetHashCode()
      {
        return this.hue << 6 | this.saturation << 2 | this.luminosity;
      }

      public Color Lighter(float percLighter)
      {
        int luminosity = this.luminosity;
        int num5 = this.NewLuma(500, true);
        return this.ColorFromHLS(this.hue, luminosity + (int)((num5 - luminosity) * percLighter), this.saturation);
      }

      private int NewLuma(int n, bool scale)
      {
        return this.NewLuma(this.luminosity, n, scale);
      }

      private int NewLuma(int luminosity, int n, bool scale)
      {
        int result;
        if (n == 0)
        {
          result = luminosity;
        }
        else if (scale)
        {
          if (n > 0)
          {
            result = (int)((luminosity * (1000 - n) + 241L * n) / 1000L);
          }
          else
          {
            result = luminosity * (n + 1000) / 1000;
          }
        }
        else
        {
          int num = luminosity + (int)(n * 240L / 1000L);
          if (num < 0)
          {
            num = 0;
          }
          if (num > 240)
          {
            num = 240;
          }
          result = num;
        }
        return result;
      }

      private Color ColorFromHLS(int hue, int luminosity, int saturation)
      {
        byte num4;
        byte num3;
        byte num2;
        if (saturation == 0)
        {
          num2 = (num3 = (num4 = (byte)(luminosity * 255 / 240)));
          if (hue == 160)
          {
          }
        }
        else
        {
          int num5;
          if (luminosity <= 120)
          {
            num5 = (luminosity * (240 + saturation) + 120) / 240;
          }
          else
          {
            num5 = luminosity + saturation - (luminosity * saturation + 120) / 240;
          }
          int num6 = 2 * luminosity - num5;
          num3 = (byte)((this.HueToRGB(num6, num5, hue + 80) * 255 + 120) / 240);
          num2 = (byte)((this.HueToRGB(num6, num5, hue) * 255 + 120) / 240);
          num4 = (byte)((this.HueToRGB(num6, num5, hue - 80) * 255 + 120) / 240);
        }
        return new Color(num3, num2, num4);
      }

      private int HueToRGB(int n1, int n2, int hue)
      {
        if (hue < 0)
        {
          hue += 240;
        }
        if (hue > 240)
        {
          hue -= 240;
        }
        int result;
        if (hue < 40)
        {
          result = n1 + ((n2 - n1) * hue + 20) / 40;
        }
        else if (hue < 120)
        {
          result = n2;
        }
        else if (hue < 160)
        {
          result = n1 + ((n2 - n1) * (160 - hue) + 20) / 40;
        }
        else
        {
          result = n1;
        }
        return result;
      }
    }

    private static Texture2D pixel;
    private static object syncRoot;

    static SpriteBatchExtensions()
    {
      syncRoot = new object();
    }

    private static void Initialize(GraphicsDevice device)
    {
      if (pixel == null)
      {
        lock (syncRoot)
        {
          if (pixel == null)
          {
            pixel = new Texture2D(device, 1, 1);
            pixel.SetData<Color>(new Color[] { Color.White });
            device.Disposing += device_Disposing;
          }
        }
      }
    }

    private static void device_Disposing(object sender, EventArgs e)
    {
      if (pixel != null)
      {
        pixel.Dispose();
        pixel = null;
      }
    }

    private static void drawLine(SpriteBatch spriteBatch, Vector2 pt0, Vector2 pt1, Color color)
    {
      float distance = Vector2.Distance(pt0, pt1);
      float angle = (float)Math.Atan2(pt1.Y - pt0.Y, pt1.X - pt0.X);
      spriteBatch.Draw(pixel, pt0, null, color, angle, Vector2.Zero, new Vector2(distance, 1f), 0, 0f);
    }

    public static void Begin(this SpriteBatch spriteBatch, Matrix transform)
    {
      spriteBatch.Begin(0, BlendState.AlphaBlend, null, null, null, null, transform);
    }

    public static void DrawLine(this SpriteBatch spriteBatch, Vector2 pt0, Vector2 pt1, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      drawLine(spriteBatch, pt0, pt1, color);
    }

    public static void DrawLine(this SpriteBatch spriteBatch, Point pt0, Point pt1, Color color)
    {
      spriteBatch.DrawLine(pt0.ToVector2(), pt1.ToVector2(), color);
    }

    public static void DrawRectangle(this SpriteBatch spriteBatch, BoxF bounds, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, new Vector2(bounds.X, bounds.Y), null, color, 0f, Vector2.Zero, new Vector2(bounds.Width, 1f), 0, 0f);
      spriteBatch.Draw(pixel, new Vector2(bounds.X, bounds.Y + (bounds.Height - 1f)), null, color, 0f, Vector2.Zero, new Vector2(bounds.Width, 1f), 0, 0f);
      spriteBatch.Draw(pixel, new Vector2(bounds.X, bounds.Y), null, color, 0f, Vector2.Zero, new Vector2(1f, bounds.Height), 0, 0f);
      spriteBatch.Draw(pixel, new Vector2(bounds.X + (bounds.Width - 1f), bounds.Y), null, color, 0f, Vector2.Zero, new Vector2(1f, bounds.Height), 0, 0f);
    }

    public static void DrawRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, new Vector2(x, y), null, color, 0f, Vector2.Zero, new Vector2(width, 1f), 0, 0f);
      spriteBatch.Draw(pixel, new Vector2(x, y + (height - 1f)), null, color, 0f, Vector2.Zero, new Vector2(width, 1f), 0, 0f);
      spriteBatch.Draw(pixel, new Vector2(x, y), null, color, 0f, Vector2.Zero, new Vector2(1f, height), 0, 0f);
      spriteBatch.Draw(pixel, new Vector2(x + (width - 1f), y), null, color, 0f, Vector2.Zero, new Vector2(1f, height), 0, 0f);
    }

    public static void DrawRectangle(this SpriteBatch spriteBatch, Rectangle bounds, Color color)
    {
      spriteBatch.DrawRectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height, color);
    }

    public static void DrawRectangle(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, new Rectangle(x, y, width, 1), color);
      spriteBatch.Draw(pixel, new Rectangle(x, y + (height - 1), width, 1), color);
      spriteBatch.Draw(pixel, new Rectangle(x, y, 1, height), color);
      spriteBatch.Draw(pixel, new Rectangle(x + (width - 1), y, 1, height), color);
    }

    public static void DrawPolygon(this SpriteBatch spriteBatch, IEnumerable<Vector2> points, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      Vector2[] polygon = points.ToArray();
      for (int i = 1; i < polygon.Length; i++)
      {
        Vector2 pt0 = polygon[i - 1];
        Vector2 pt = polygon[i];
        drawLine(spriteBatch, pt0, pt, color);
      }
    }

    public static void DrawPolygon(this SpriteBatch spriteBatch, IEnumerable<Point> points, Color color)
    {
      spriteBatch.DrawPolygon(
        from p in points
        select p.ToVector2(), color);
    }

    public static void FillRectangle(this SpriteBatch spriteBatch, BoxF bounds, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, bounds.Location, null, color, 0f, Vector2.Zero, bounds.Size.ToVector2(), 0, 0f);
    }

    public static void FillRectangle(this SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
    {
      spriteBatch.FillRectangle(new BoxF(x, y, width, height), color);
    }

    public static void FillRectangle(this SpriteBatch spriteBatch, Rectangle bounds, Color color)
    {
      Initialize(spriteBatch.GraphicsDevice);
      spriteBatch.Draw(pixel, bounds, color);
    }

    public static void FillRectangle(this SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
      spriteBatch.FillRectangle(new Rectangle(x, y, width, height), color);
    }

    public static Point Offset(this Point point, int dx, int dy)
    {
      return new Point(point.X + dx, point.Y + dy);
    }

    public static Vector2 DropZ(this Vector3 value)
    {
      return new Vector2(value.X, value.Y);
    }

    public static Vector2 GetSize(this Viewport viewport)
    {
      return new Vector2((float)viewport.Width, (float)viewport.Height);
    }

    public static Rectangle GetRectangle(this Viewport viewport)
    {
      return new Rectangle(viewport.X, viewport.Y, viewport.Width, viewport.Height);
    }

    public static Point Center(this Rectangle rect)
    {
      return new Point
      {
        X = rect.X + rect.Width / 2,
        Y = rect.Y + rect.Height / 2
      };
    }

    public static Point NextPoint(this Random random, Rectangle rect)
    {
      return new Point
      {
        X = random.Next(rect.Left, rect.Right),
        Y = random.Next(rect.Top, rect.Bottom)
      };
    }

    public static Point NextPoint(this Random random, Point min, Point max)
    {
      return new Point
      {
        X = random.Next(min.X, max.X),
        Y = random.Next(min.Y, max.Y)
      };
    }

    public static int Round(this float f)
    {
      return (int)Math.Round(f);
    }

    public static Point Round(this Vector2 v)
    {
      return new Point(v.X.Round(), v.Y.Round());
    }

    public static IEnumerable<Point> Round(this IEnumerable<Vector2> vecs)
    {
      return
        from vec in vecs
        select vec.Round();
    }

    public static int Floor(this float f)
    {
      return (int)Math.Floor(f);
    }

    public static Point Floor(this Vector2 v)
    {
      return new Point(v.X.Floor(), v.Y.Floor());
    }

    public static IEnumerable<Point> Floor(this IEnumerable<Vector2> vecs)
    {
      return
        from vec in vecs
        select vec.Floor();
    }

    public static int Ceiling(this float f)
    {
      return (int)Math.Ceiling(f);
    }

    public static Point Ceiling(this Vector2 v)
    {
      return new Point(v.X.Ceiling(), v.Y.Ceiling());
    }

    public static IEnumerable<Point> Ceiling(this IEnumerable<Vector2> vecs)
    {
      return
        from vec in vecs
        select vec.Ceiling();
    }

    public static Vector2 ToVector2(this Point pt)
    {
      return new Vector2(pt.X, pt.Y);
    }

    public static BoxF GetBounds(IEnumerable<Vector2> points)
    {
      float left = float.MaxValue;
      float top = float.MaxValue;
      float right = float.MinValue;
      float bottom = float.MinValue;

      foreach (Vector2 p in points)
      {
        left = Math.Min(left, p.X);
        top = Math.Min(top, p.Y);
        right = Math.Max(right, p.X);
        bottom = Math.Max(bottom, p.Y);
      }
      return BoxF.FromLTRB(left, top, right, bottom);
    }

    public static Vector3 ToVector3(this Vector2 vec)
    {
      return new Vector3(vec, 0f);
    }

    public static Vector3 ToVector3(this Vector2 vec, float z)
    {
      return new Vector3(vec, z);
    }

    public static Color AlphaBlend(this Color current, Color color)
    {
      Color retval = current;
      Vector4 dest = current.ToVector4();
      Vector4 src = color.ToVector4();
      float Sa = src.W;
      float Da = dest.W;
      if (Sa > 0f)
      {
        float invSa = 1f - Sa;
        float daInvSa = Da * invSa;
        float Ra = Sa + daInvSa;
        Vector3 Sc = new Vector3(src.X, src.Y, src.Z);
        Vector3 Dc = new Vector3(dest.X, dest.Y, dest.Z);
        Vector3 left;
        Vector3.Multiply(ref Sc, Sa, out left);
        Vector3 right;
        Vector3.Multiply(ref Dc, daInvSa, out right);
        Vector3 result;
        Vector3.Add(ref left, ref right, out result);
        Vector3 Rc;
        Vector3.Divide(ref result, Ra, out Rc);
        retval = new Color(new Vector4(Rc, Ra));
      }
      return retval;
    }

    public static Color BlendWith(this Color color1, Color color2, float factor)
    {
      Color c1 = color1;
      Color c2 = color2;
      float pc = MathHelper.Clamp(factor, 0f, 1f);

      int c1a = (int)c1.A;
      int c1r = (int)c1.R;
      int c1g = (int)c1.G;
      int c1b = (int)c1.B;

      int c2a = (int)c2.A;
      int c2r = (int)c2.R;
      int c2g = (int)c2.G;
      int c2b = (int)c2.B;

      int a = (int)Math.Abs(c1a - (c1a - c2a) * pc);
      int r = (int)Math.Abs(c1r - (c1r - c2r) * pc);
      int g = (int)Math.Abs(c1g - (c1g - c2g) * pc);
      int b = (int)Math.Abs(c1b - (c1b - c2b) * pc);

      return new Color(
        (byte)MathHelper.Clamp(r, 0f, 255f), 
        (byte)MathHelper.Clamp(g, 0f, 255f),
        (byte)MathHelper.Clamp(b, 0f, 255f), 
        (byte)MathHelper.Clamp(a, 0f, 255f));
    }

    public static Color Clone(this Color color)
    {
      return new Color(color.ToVector4());
    }

    public static Color Opposite(this Color color)
    {
      return new Color(255 - color.R, 255 - color.G, 255 - color.B);
    }

    public static Color Light(this Color baseColor)
    {
      HLSColor color = new HLSColor(baseColor);
      return color.Lighter(0.5f);
    }

    public static Color Dark(this Color baseColor)
    {
      HLSColor color = new HLSColor(baseColor);
      return color.Darker(0.5f);
    }

    public static Color Alpha(this Color color, int alpha)
    {
      return new Color((int)color.R, (int)color.G, (int)color.B,
        (byte)MathHelper.Clamp(alpha, 0f, 255f));
    }

    public static Color Alpha(this Color color, byte alpha)
    {
      return new Color((int)color.R, (int)color.G, (int)color.B,
        (byte)MathHelper.Clamp(alpha, 0f, 255f));
    }

    public static Color Alpha(this Color color, float alpha)
    {
      return new Color((int)color.R, (int)color.G, (int)color.B,
        (byte)MathHelper.Clamp(alpha * 255f, 0f, 255f));
    }

    public static Color Mult(this Color color1, Color color)
    {
      Vector4 a = color1.ToVector4();
      Vector4 b = color.ToVector4();
      return new Color(a * b);
    }

    public static Vector2 GetTransformedPosition(this MouseState state, Matrix inverseTransform)
    {
      return Vector2.Transform(new Vector2((float)state.X, (float)state.Y), inverseTransform);
    }

    public static Texture2D MakeTransparent(this Texture2D texture, Color color)
    {
      Color[] data = texture.GetPixels();
      for (int i = 0; i < data.Length; i++)
      {
        if (data[i].Equals(color))
        {
          data[i] = default(Color);
        }
      }
      Texture2D retval = new Texture2D(texture.GraphicsDevice, texture.Width, texture.Height, true, texture.Format);
      retval.SetData<Color>(data);
      return retval;
    }

    public static Color[] GetPixels(this Texture2D texture)
    {
      return texture.GetPixels(texture.Bounds());
    }

    public static Color[] GetPixels(this Texture2D texture, Box source)
    {
      Color[] data = new Color[source.Width * source.Height];
      texture.GetData<Color>(0, new Rectangle?(source.ToRectangle()), data, 0, data.Length);
      return data;
    }

    public static Box Bounds(this Texture2D texture2D)
    {
      return new Box(0, 0, texture2D.Width, texture2D.Height);
    }

    public static Size Size(this Texture2D texture2D)
    {
      return new Size(texture2D.Width, texture2D.Height);
    }

    public static void To480p(this GraphicsDeviceManager graphics)
    {
      graphics.PreferredBackBufferWidth = 720;
      graphics.PreferredBackBufferHeight = 480;
    }

    public static void To720p(this GraphicsDeviceManager graphics)
    {
      graphics.PreferredBackBufferWidth = 1280;
      graphics.PreferredBackBufferHeight = 720;
    }

    public static void To1080p(this GraphicsDeviceManager graphics)
    {
      graphics.PreferredBackBufferWidth = 1920;
      graphics.PreferredBackBufferHeight = 1080;
    }
  }
}