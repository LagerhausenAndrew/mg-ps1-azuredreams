using System;
using Microsoft.Xna.Framework;

namespace AzureDreams.MonoDirectX
{
  /// <summary>
  /// Stores a set of four floating-point numbers that represent the location and size of a box.
  /// </summary>
  [Serializable]
  public struct BoxF
  {
    /// <summary>
    /// Represents an uninitialized instance of a BoxF.
    /// </summary>
    public static readonly BoxF Empty;

    private float x;
    private float y;
    private float width;
    private float height;

    /// <summary>
    /// Creates a new instance of the BoxF class with the specified location and size.
    /// </summary>
    /// <param name="location">The upper-left corner of the box.</param>
    /// <param name="size">The size of the box.</param>
    public BoxF(Vector2 location, SizeF size)
      : this(location.X, location.Y, size.Width, size.Height)
    {

    }

    /// <summary>
    /// Creates a new instance of the BoxF class with the specified location and size.
    /// </summary>
    /// <param name="location">The upper-left corner of the box.</param>
    /// <param name="size">The size of the box.</param>
    public BoxF(Vector2 location, Vector2 size)
      : this(location.X, location.Y, size.X, size.Y)
    {

    }

    /// <summary>
    /// Creates a new instance of the BoxF class with the specified location and size.
    /// </summary>
    /// <param name="location">The upper-left corner of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    public BoxF(Vector2 location, float width, float height)
      : this(location.X, location.Y, width, height)
    {

    }

    /// <summary>
    /// Creates a new instance of the BoxF class with the specified location and size.
    /// </summary>
    /// <param name="x">The x-coordinate of the upper-left corner of the box.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    public BoxF(float x, float y, float width, float height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    /// <summary>
    /// Creates a BoxF structure with upper-left corner and lower-right corner at the specified locations.
    /// </summary>
    /// <param name="left">The x-coordinate of the upper-left corner of the box.</param>
    /// <param name="top">The y-coordinate of the upper-left corner of the box.</param>
    /// <param name="right">The x-coordinate of the lower-right corner of the box.</param>
    /// <param name="bottom">The y-coordinate of the lower-right corner of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromLTRB(float left, float top, float right, float bottom)
    {
      return new BoxF(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Creates a BoxF structre with the specified center and size.
    /// </summary>
    /// <param name="center">The center of the box.</param>
    /// <param name="size">The size of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromCenterAndSize(Vector2 center, Vector2 size)
    {
      return FromCenterAndSize(center.X, center.Y, size.X, size.Y);
    }

    /// <summary>
    /// Creates a BoxF structre with the specified center and size.
    /// </summary>
    /// <param name="center">The center of the box.</param>
    /// <param name="size">The size of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromCenterAndSize(Vector2 center, SizeF size)
    {
      return FromCenterAndSize(center.X, center.Y, size.Width, size.Height);
    }

    /// <summary>
    /// Creates a BoxF structre with the specified center and size.
    /// </summary>
    /// <param name="cx">The center x-coordinate of the box.</param>
    /// <param name="cy">The center y-coordinate of the box.</param>
    /// <param name="size">The size of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromCenterAndSize(float cx, float cy, Vector2 size)
    {
      return FromCenterAndSize(cx, cy, size.X, size.Y);
    }

    /// <summary>
    /// Creates a BoxF structre with the specified center and size.
    /// </summary>
    /// <param name="cx">The center x-coordinate of the box.</param>
    /// <param name="cy">The center y-coordinate of the box.</param>
    /// <param name="size">The size of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromCenterAndSize(float cx, float cy, SizeF size)
    {
      return FromCenterAndSize(cx, cy, size.Width, size.Height);
    }

    /// <summary>
    /// Creates a BoxF structre with the specified center and size.
    /// </summary>
    /// <param name="center">The center of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromCenterAndSize(Vector2 center, float width, float height)
    {
      return FromCenterAndSize(center.X, center.Y, width, height);
    }

    /// <summary>
    /// Creates a BoxF structre with the specified center and size.
    /// </summary>
    /// <param name="cx">The center x-coordinate of the box.</param>
    /// <param name="cy">The center y-coordinate of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static BoxF FromCenterAndSize(float cx, float cy, float width, float height)
    {
      BoxF retval = new BoxF(0, 0, width, height);
      retval.Center = new Vector2(cx, cy);
      return retval;
    }

    /// <summary>
    /// Gets or sets the coordinates of the upper-left corner of this BoxF.
    /// </summary>
    public Vector2 Location
    {
      get 
      { 
        return new Vector2(this.X, this.Y); 
      }
      set
      { 
        this.X = value.X;
        this.Y = value.Y;
      }
    }

    /// <summary>
    /// Gets or sets the size of this BoxF.
    /// </summary>
    public SizeF Size
    {
      get
      {
        return new SizeF(this.Width, this.Height);
      }
      set
      {
        this.Width = value.Width;
        this.Height = value.Height;
      }
    }

    /// <summary>
    /// Gets or sets the upper-left x-coordinate of this BoxF.
    /// </summary>
    public float X
    {
      get { return this.x; }
      set { this.x = value; }
    }

    /// <summary>
    /// Gets or sets the upper-left y-coordinate of this BoxF.
    /// </summary>
    public float Y
    {
      get { return this.y; }
      set { this.y = value; }
    }

    /// <summary>
    /// Gets or set the width of this BoxF.
    /// </summary>
    public float Width
    {
      get { return this.width; }
      set { this.width = value; }
    }

    /// <summary>
    /// Gets or sets the height of this BoxF.
    /// </summary>
    public float Height
    {
      get { return this.height; }
      set { this.height = value; }
    }

    /// <summary>
    /// Gets the upper-left x-coordinate of this BoxF.
    /// </summary>
    public float Left
    {
      get { return this.X; }
    }

    /// <summary>
    /// Gets the upper-left y-coordinate of this BoxF.
    /// </summary>
    public float Top
    {
      get { return this.Y; }
    }

    /// <summary>
    /// Gets the lower-right x-coordinate of this BoxF.
    /// </summary>
    public float Right
    {
      get { return (this.X + this.Width); }
    }

    /// <summary>
    /// Gets the lower-right y-coordinate of this BoxF.
    /// </summary>
    public float Bottom
    {
      get { return (this.Y + this.Height); }
    }

    /// <summary>
    /// Gets the center of this Box.
    /// </summary>
    public Vector2 Center
    {
      get
      {
        return new Vector2((X + Right) / 2, (Y + Bottom) / 2);
      }
      set
      {
        Location += value - Center;
      }
    }

    /// <summary>
    /// Tests whether the Width or Height property of this BoxF has a less than or equal to 0.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        if (this.Width > 0f)
        {
          return (this.Height <= 0f);
        }
        return true;
      }
    }

    /// <summary>
    /// Tests whether obj is a BoxF with the same location and size of this BoxF.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>A value indicating if the object matches this BoxF.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is BoxF))
      {
        return false;
      }
      BoxF ef = (BoxF)obj;
      return ((((ef.X == this.X) && (ef.Y == this.Y)) && (ef.Width == this.Width)) && (ef.Height == this.Height));
    }

    /// <summary>
    /// Tests whether two BoxF structures have equal location and size.
    /// </summary>
    /// <param name="left">The box on the left side of the equality equation.</param>
    /// <param name="right">The box on the right side of the equality equation.</param>
    /// <returns>A value indicating if the two boxes are equal.</returns>
    public static bool operator ==(BoxF left, BoxF right)
    {
      return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
    }

    /// <summary>
    /// Tests whether two BoxF structures don't have equal location and size.
    /// </summary>
    /// <param name="left">The box on the left side of the equality equation.</param>
    /// <param name="right">The box on the right side of the equality equation.</param>
    /// <returns>A value indicating if the two boxes aren't equal.</returns>
    public static bool operator !=(BoxF left, BoxF right)
    {
      return !(left == right);
    }

    /// <summary>
    /// Determines if the specified point is contained within this BoxF structure.
    /// </summary>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    /// <returns>A value indicating if this box contains the point.</returns>
    public bool Contains(float x, float y)
    {
      return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
    }

    /// <summary>
    /// Determines if the specified point is contained within this BoxF structure.
    /// </summary>
    /// <param name="pt">The point to test.</param>
    /// <returns>A value indicating if this box contains the point.</returns>
    public bool Contains(Vector2 pt)
    {
      return this.Contains(pt.X, pt.Y);
    }

    /// <summary>
    /// Determines if the specified point is contained within this BoxF structure.
    /// </summary>
    /// <param name="pt">The point to test.</param>
    /// <returns>A value indicating if this box contains the point.</returns>
    public bool Contains(Point pt)
    {
      return this.Contains(pt.X, pt.Y);
    }

    /// <summary>
    /// Determines if the rectangular region represented by box is entirely contained within this BoxF structure.
    /// </summary>
    /// <param name="box">The box to test.</param>
    /// <returns>A value indicating if this box contains the box.</returns>
    public bool Contains(BoxF box)
    {
      return ((((this.X <= box.X) && ((box.X + box.Width) <= (this.X + this.Width))) && (this.Y <= box.Y)) && ((box.Y + box.Height) <= (this.Y + this.Height)));
    }

    /// <summary>
    /// Gets the hash code for this BoxF structure.
    /// </summary>
    /// <returns>The has code of this box.</returns>
    public override int GetHashCode()
    {
      return (int)(((((uint)this.X) ^ ((((uint)this.Y) << 13) | (((uint)this.Y) >> 0x13))) ^ ((((uint)this.Width) << 0x1a) | (((uint)this.Width) >> 6))) ^ ((((uint)this.Height) << 7) | (((uint)this.Height) >> 0x19)));
    }

    /// <summary>
    /// Inflates this BoxF structure by the specified amount. This will adjust the x and y values to grow (or shrink), and then
    /// adjust the width and height to match.
    /// </summary>
    /// <param name="x">The amount to inflate horizontally.</param>
    /// <param name="y">The amount to inflate vertically.</param>
    public void Inflate(float x, float y)
    {
      this.X -= x;
      this.Y -= y;
      this.Width += 2f * x;
      this.Height += 2f * y;
    }

    /// <summary>
    /// Inflates this BoxF structure by the specified amount. This will adjust the x and y values to grow (or shrink), and then
    /// adjust the width and height to match.
    /// </summary>
    /// <param name="size">The amount to inflate this box.</param>
    public void Inflate(SizeF size)
    {
      this.Inflate(size.Width, size.Height);
    }

    /// <summary>
    /// Returns an inflated copy of the box.
    /// </summary>
    /// <param name="box">The box to copy and inflate. This is not modified.</param>
    /// <param name="x">The amount to inflate horizontally.</param>
    /// <param name="y">The amount to inflate vertically.</param>
    /// <returns>A newly created inflated box.</returns>
    public static BoxF Inflate(BoxF box, float x, float y)
    {
      BoxF ef = box;
      ef.Inflate(x, y);
      return ef;
    }

    /// <summary>
    /// Replaces this BoxF structure with the intersection of itself and the specified BoxF structure.
    /// </summary>
    /// <param name="box">The box to intersect.</param>
    public void Intersect(BoxF box)
    {
      BoxF ef = Intersect(box, this);
      this.X = ef.X;
      this.Y = ef.Y;
      this.Width = ef.Width;
      this.Height = ef.Height;
    }

    /// <summary>
    /// Converts this floating-point box to an integer box.
    /// </summary>
    /// <returns>This BoxF as a Box</returns>
    public Box ToBox()
    {
      return new Box
      {
        X = (int)Math.Round(X),
        Y = (int)Math.Round(Y),
        Width = (int)Math.Round(Width),
        Height = (int)Math.Round(Height),
      };
    }

    /// <summary>
    /// Returns a BoxF structure that represents the intersection of two boxes. If there is no intersection, an empty BoxF is returned.
    /// </summary>
    /// <param name="a">The first box to intersect.</param>
    /// <param name="b">The second box to intersect.</param>
    /// <returns>A box that represents the intersection of two boxes.</returns>
    public static BoxF Intersect(BoxF a, BoxF b)
    {
      float x = Math.Max(a.X, b.X);
      float num2 = Math.Min((float)(a.X + a.Width), (float)(b.X + b.Width));
      float y = Math.Max(a.Y, b.Y);
      float num4 = Math.Min((float)(a.Y + a.Height), (float)(b.Y + b.Height));
      if ((num2 >= x) && (num4 >= y))
      {
        return new BoxF(x, y, num2 - x, num4 - y);
      }
      return Empty;
    }

    /// <summary>
    /// Determines if this box intersects with another box.
    /// </summary>
    /// <param name="box">The box to test.</param>
    /// <returns>A value indicating if the two boxes intersect.</returns>
    public bool IntersectsWith(BoxF box)
    {
      return ((((box.X < (this.X + this.Width)) && (this.X < (box.X + box.Width))) && (box.Y < (this.Y + this.Height))) && (this.Y < (box.Y + box.Height)));
    }

    /// <summary>
    /// Creates the smallest possible box that can contain both of two box that form a union.
    /// </summary>
    /// <param name="a">The first box to union.</param>
    /// <param name="b">The second box to union.</param>
    /// <returns>A box that represents the union of two boxes.</returns>
    public static BoxF Union(BoxF a, BoxF b)
    {
      float x = Math.Min(a.X, b.X);
      float num2 = Math.Max((float)(a.X + a.Width), (float)(b.X + b.Width));
      float y = Math.Min(a.Y, b.Y);
      float num4 = Math.Max((float)(a.Y + a.Height), (float)(b.Y + b.Height));
      return new BoxF(x, y, num2 - x, num4 - y);
    }

    /// <summary>
    /// Translates the upper-left coordinate of this box by the specified point.
    /// </summary>
    /// <param name="pos">A point representing the amount to translate in the x and y direction.</param>
    public void Offset(Vector2 pos)
    {
      this.Offset(pos.X, pos.Y);
    }

    /// <summary>
    /// Translates the upper-left coordinate of this box by the specified point.
    /// </summary>
    /// <param name="x">The amount to translate in the x direction.</param>
    /// <param name="y">The amount to translate in the y direction.</param>
    public void Offset(float x, float y)
    {
      this.X += x;
      this.Y += y;
    }

    /// <summary>
    /// Translates the upper-left coordinate of the box by the specified point.
    /// </summary>
    /// <param name="box">The box to adjust. This is not affected.</param>
    /// <param name="dx">The amount to translate in the x direction.</param>
    /// <param name="dy">The amount to translate in the y direction.</param>
    /// <returns>A new translated box.</returns>
    public static BoxF Offset(BoxF box, float dx, float dy)
    {
      return new BoxF(box.X + dx, box.Y + dy, box.Width, box.height);
    }

    /// <summary>
    /// Translates the upper-left coordinate of the box by the specified point.
    /// </summary>
    /// <param name="box">The box to adjust. This is not affected.</param>
    /// <param name="offset">The amount to translate in the x and y direction.</param>
    /// <returns>A new translated box.</returns>
    public static BoxF Offset(BoxF box, Vector2 offset)
    {
      return new BoxF(box.Location + offset, box.Size);
    }

    /// <summary>
    /// Converts the specified Rectangle structure to a BoxF structure.
    /// </summary>
    /// <param name="r">The rectangle to convert.</param>
    /// <returns>The converted box.</returns>
    public static implicit operator BoxF(Rectangle r)
    {
      return new BoxF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
    }

    /// <summary>
    /// Converts the specified Box structure to a BoxF structure.
    /// </summary>
    /// <param name="r">The box to convert.</param>
    /// <returns>The converted box.</returns>
    public static implicit operator BoxF(Box r)
    {
      return new BoxF((float)r.X, (float)r.Y, (float)r.Width, (float)r.Height);
    }

    /// <summary>
    /// Converts the Location and Size of this BoxF to a human-readable string.
    /// </summary>
    /// <returns>A string that contains the Location and Size.</returns>
    public override string ToString()
    {
      return ("{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + ",Width=" + this.Width.ToString() + ",Height=" + this.Height.ToString() + "}");
    }

    static BoxF()
    {
      Empty = new BoxF();
    }
  }
}
