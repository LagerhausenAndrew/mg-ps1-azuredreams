using System;
using Microsoft.Xna.Framework;

namespace AzureDreams.MonoDirectX
{
  /// <summary>
  /// Stores a set of four floating-point numbers that represent the location and size of a box.
  /// </summary>
  [Serializable]
  public struct Box
  {
    /// <summary>
    /// Represents an uninitialized instance of a Box.
    /// </summary>
    public static readonly Box Empty;

    private int x;
    private int y;
    private int width;
    private int height;

    /// <summary>
    /// Creates a new instance of the Box class with the specified location and size.
    /// </summary>
    /// <param name="location">The upper-left corner of the box.</param>
    /// <param name="size">The size of the box.</param>
    public Box(Point location, Size size)
      : this(location.X, location.Y, size.Width, size.Height)
    {

    }

    /// <summary>
    /// Creates a new instance of the Box class with the specified rectangle.
    /// </summary>
    /// <param name="rectangle">The rectangle to copy over.</param>
    public Box(Rectangle rectangle)
      : this(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height)
    {

    }

    /// <summary>
    /// reates a new instance of the Box class with the specified location and size.
    /// </summary>
    /// <param name="x">The x-coordinate of the upper-left corner of the box.</param>
    /// <param name="y">The y-coordinate of the upper-left corner of the box.</param>
    /// <param name="width">The width of the box.</param>
    /// <param name="height">The height of the box.</param>
    public Box(int x, int y, int width, int height)
    {
      this.x = x;
      this.y = y;
      this.width = width;
      this.height = height;
    }

    /// <summary>
    /// Creates a Box structure with upper-left corner and lower-right corner at the specified locations.
    /// </summary>
    /// <param name="left">The x-coordinate of the upper-left corner of the box.</param>
    /// <param name="top">The y-coordinate of the upper-left corner of the box.</param>
    /// <param name="right">The x-coordinate of the lower-right corner of the box.</param>
    /// <param name="bottom">The y-coordinate of the lower-right corner of the box.</param>
    /// <returns>A new box created from the coordinates.</returns>
    public static Box FromLTRB(int left, int top, int right, int bottom)
    {
      return new Box(left, top, right - left, bottom - top);
    }

    /// <summary>
    /// Gets or sets the coordinates of the upper-left corner of this Box.
    /// </summary>
    public Point Location
    {
      get
      {
        return new Point(this.X, this.Y);
      }
      set
      {
        this.X = value.X;
        this.Y = value.Y;
      }
    }

    /// <summary>
    /// Gets or sets the size of this Box.
    /// </summary>
    public Size Size
    {
      get
      {
        return new Size(this.Width, this.Height);
      }
      set
      {
        this.Width = value.Width;
        this.Height = value.Height;
      }
    }

    /// <summary>
    /// Gets or sets the upper-left x-coordinate of this Box.
    /// </summary>
    public int X
    {
      get { return this.x; }
      set { this.x = value; }
    }

    /// <summary>
    /// Gets or sets the upper-left y-coordinate of this Box.
    /// </summary>
    public int Y
    {
      get { return this.y; }
      set { this.y = value;}
    }

    /// <summary>
    /// Gets or set the width of this Box.
    /// </summary>
    public int Width
    {
      get { return this.width; }
      set { this.width = value; }
    }

    /// <summary>
    /// Gets or sets the height of this Box.
    /// </summary>
    public int Height
    {
      get { return this.height; }
      set { this.height = value; }
    }

    /// <summary>
    /// Gets the upper-left x-coordinate of this Box.
    /// </summary>
    public int Left
    {
      get { return this.X; }
    }

    /// <summary>
    /// Gets the upper-left y-coordinate of this Box.
    /// </summary>
    public int Top
    {
      get { return this.Y; }
    }

    /// <summary>
    /// Gets the lower-right x-coordinate of this Box.
    /// </summary>
    public int Right
    {
      get { return (this.X + this.Width); }
    }

    /// <summary>
    /// Gets the lower-right y-coordinate of this Box.
    /// </summary>
    public int Bottom
    {
      get { return (this.Y + this.Height); }
    }

    /// <summary>
    /// Gets the center of this Box.
    /// </summary>
    public Point Center
    {
      get
      {
        return new Point((X + Right) / 2, (Y + Bottom) / 2);
      }
      set
      {
        Point center = Center;
        Point location = new Point(X + (value.X - center.X), Y + (value.Y - center.Y));
        Location = location;
      }
    }

    /// <summary>
    /// Tests whether the Width or Height property of this Box has a less than or equal to 0.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        if (this.Width > 0)
        {
          return (this.Height <= 0);
        }
        return true;
      }
    }

    /// <summary>
    /// Tests whether obj is a Box with the same location and size of this Box.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>A value indicating if the object matches this Box.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is Box))
      {
        return false;
      }
      Box rectangle = (Box)obj;
      return ((((rectangle.X == this.X) && (rectangle.Y == this.Y)) && (rectangle.Width == this.Width)) && (rectangle.Height == this.Height));
    }

    /// <summary>
    /// Tests whether two Box structures have equal location and size.
    /// </summary>
    /// <param name="left">The box on the left side of the equality equation.</param>
    /// <param name="right">The box on the right side of the equality equation.</param>
    /// <returns>A value indicating if the two boxes are equal.</returns>
    public static bool operator ==(Box left, Box right)
    {
      return ((((left.X == right.X) && (left.Y == right.Y)) && (left.Width == right.Width)) && (left.Height == right.Height));
    }

    /// <summary>
    /// Tests whether two Box structures don't have equal location and size.
    /// </summary>
    /// <param name="left">The box on the left side of the equality equation.</param>
    /// <param name="right">The box on the right side of the equality equation.</param>
    /// <returns>A value indicating if the two boxes aren't equal.</returns>
    public static bool operator !=(Box left, Box right)
    {
      return !(left == right);
    }

    /// <summary>
    /// Converts a Box to a XNA Rectangle.
    /// </summary>
    /// <param name="box">The box to convert.</param>
    /// <returns>The box as a rectangle.</returns>
    public static implicit operator Rectangle(Box box)
    {
      return box.ToRectangle();
    }

    /// <summary>
    /// Converts a XNA Rectangle to a Box.
    /// </summary>
    /// <param name="box">The box to convert.</param>
    /// <returns>The box as a rectangle.</returns>
    public static implicit operator Box(Rectangle box)
    {
      return new Box(box.X, box.Y, box.Width, box.Height);
    }

    /// <summary>
    /// Converts the specified BoxF structure to a Box structure by rounding the BoxF values to the 
    /// next higher integer values.
    /// </summary>
    /// <param name="value">The box to be converted.</param>
    /// <returns>A converted box.</returns>
    public static Box Ceiling(BoxF value)
    {
      return new Box((int)Math.Ceiling((double)value.X), (int)Math.Ceiling((double)value.Y), (int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
    }

    /// <summary>
    /// Converts the specified BoxF to a Box by truncating the BoxF values.
    /// </summary>
    /// <param name="value">The box to be converted.</param>
    /// <returns>A converted box.</returns>
    public static Box Truncate(BoxF value)
    {
      return new Box((int)value.X, (int)value.Y, (int)value.Width, (int)value.Height);
    }

    /// <summary>
    /// Converts the specified BoxF to a Box by rounding the BoxF values to the nearest integer values.
    /// </summary>
    /// <param name="value">The box to be converted.</param>
    /// <returns>A converted box.</returns>
    public static Box Round(BoxF value)
    {
      return new Box((int)Math.Round((double)value.X), (int)Math.Round((double)value.Y), (int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
    }

    /// <summary>
    /// Determines if the specified point is contained within this box.
    /// </summary>
    /// <param name="x">The x-coordinate of the point to test.</param>
    /// <param name="y">The y-coordinate of the point to test.</param>
    /// <returns>A value indicating if the point is inside this box</returns>
    public bool Contains(int x, int y)
    {
      return ((((this.X <= x) && (x < (this.X + this.Width))) && (this.Y <= y)) && (y < (this.Y + this.Height)));
    }

    /// <summary>
    /// Determines if the specified point is contained within this box.
    /// </summary>
    /// <param name="pt">The point to test.</param>
    /// <returns>A value indicating if the point is inside this box</returns>
    public bool Contains(Point pt)
    {
      return this.Contains(pt.X, pt.Y);
    }

    /// <summary>
    /// Determines if the rectangular region represented by box is entirely contained within this Box.
    /// </summary>
    /// <param name="box">The box to test.</param>
    /// <returns>A value indicating if this box contains the box.</returns>
    public bool Contains(Box box)
    {
      return ((((this.X <= box.X) && ((box.X + box.Width) <= (this.X + this.Width))) && (this.Y <= box.Y)) && ((box.Y + box.Height) <= (this.Y + this.Height)));
    }

    /// <summary>
    /// Gets the hash code for this Box structure.
    /// </summary>
    /// <returns>The has code of this box.</returns>
    public override int GetHashCode()
    {
      return (((this.X ^ ((this.Y << 13) | (this.Y >> 0x13))) ^ ((this.Width << 0x1a) | (this.Width >> 6))) ^ ((this.Height << 7) | (this.Height >> 0x19)));
    }

    /// <summary>
    /// Inflates this Box structure by the specified amount. This will adjust the x and y values to grow (or shrink), and then
    /// adjust the width and height to match.
    /// </summary>
    /// <param name="x">The amount to inflate horizontally.</param>
    /// <param name="y">The amount to inflate vertically.</param>
    public void Inflate(int x, int y)
    {
      this.X -= x;
      this.Y -= y;
      this.Width += 2 * x;
      this.Height += 2 * y;
    }

    /// <summary>
    /// Inflates this Box structure by the specified amount. This will adjust the x and y values to grow (or shrink), and then
    /// adjust the width and height to match.
    /// </summary>
    /// <param name="size">The amount to inflate this box.</param>
    public void Inflate(Size size)
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
    public static Box Inflate(Box box, int x, int y)
    {
      Box rectangle = box;
      rectangle.Inflate(x, y);
      return rectangle;
    }

    /// <summary>
    /// Converts this Box to a XNA Rectangle.
    /// </summary>
    /// <returns>The box as a rectangle.</returns>
    public Rectangle ToRectangle()
    {
      return new Rectangle(X, Y, Width, Height);
    }

    /// <summary>
    /// Replaces this Box structure with the intersection of itself and the specified Box structure.
    /// </summary>
    /// <param name="box">The box to intersect.</param>
    public void Intersect(Box box)
    {
      Box rectangle = Intersect(box, this);
      this.X = rectangle.X;
      this.Y = rectangle.Y;
      this.Width = rectangle.Width;
      this.Height = rectangle.Height;
    }

    /// <summary>
    /// Returns a Box structure that represents the intersection of two boxes. If there is no intersection, an empty Box is returned.
    /// </summary>
    /// <param name="a">The first box to intersect.</param>
    /// <param name="b">The second box to intersect.</param>
    /// <returns>A box that represents the intersection of two boxes.</returns>
    public static Box Intersect(Box a, Box b)
    {
      int x = Math.Max(a.X, b.X);
      int num2 = Math.Min((int)(a.X + a.Width), (int)(b.X + b.Width));
      int y = Math.Max(a.Y, b.Y);
      int num4 = Math.Min((int)(a.Y + a.Height), (int)(b.Y + b.Height));
      if ((num2 >= x) && (num4 >= y))
      {
        return new Box(x, y, num2 - x, num4 - y);
      }
      return Empty;
    }

    /// <summary>
    /// Determines if this box intersects with another box.
    /// </summary>
    /// <param name="box">The box to test.</param>
    /// <returns>A value indicating if the two boxes intersect.</returns>
    public bool IntersectsWith(Box box)
    {
      return ((((box.X < (this.X + this.Width)) && (this.X < (box.X + box.Width))) && (box.Y < (this.Y + this.Height))) && (this.Y < (box.Y + box.Height)));
    }

    /// <summary>
    /// Creates the smallest possible box that can contain both of two box that form a union.
    /// </summary>
    /// <param name="a">The first box to union.</param>
    /// <param name="b">The second box to union.</param>
    /// <returns>A box that represents the union of two boxes.</returns>
    public static Box Union(Box a, Box b)
    {
      int x = Math.Min(a.X, b.X);
      int num2 = Math.Max((int)(a.X + a.Width), (int)(b.X + b.Width));
      int y = Math.Min(a.Y, b.Y);
      int num4 = Math.Max((int)(a.Y + a.Height), (int)(b.Y + b.Height));
      return new Box(x, y, num2 - x, num4 - y);
    }

    /// <summary>
    /// Translates the upper-left coordinate of this box by the specified point.
    /// </summary>
    /// <param name="pos">A point representing the amount to translate in the x and y direction.</param>
    public void Offset(Point pos)
    {
      this.Offset(pos.X, pos.Y);
    }

    /// <summary>
    /// Translates the upper-left coordinate of this box by the specified point.
    /// </summary>
    /// <param name="x">The amount to translate in the x direction.</param>
    /// <param name="y">The amount to translate in the y direction.</param>
    public void Offset(int x, int y)
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
    public static Box Offset(Box box, int dx, int dy)
    {
      return new Box(box.X + dx, box.Y + dy, box.Width, box.height);
    }

    /// <summary>
    /// Translates the upper-left coordinate of the box by the specified point.
    /// </summary>
    /// <param name="box">The box to adjust. This is not affected.</param>
    /// <param name="offset">The amount to translate in the x and y direction.</param>
    /// <returns>A new translated box.</returns>
    public static Box Offset(Box box, Point offset)
    {
      return new Box(box.X + offset.X, box.Y + offset.Y, box.Width, box.Height);
    }

    /// <summary>
    /// Converts the Location and Size of this BoxF to a human-readable string.
    /// </summary>
    /// <returns>A string that contains the Location and Size.</returns>
    public override string ToString()
    {
      return ("{X=" + this.X.ToString() + ",Y=" + this.Y.ToString() + ",Width=" + this.Width.ToString() + ",Height=" + this.Height.ToString() + "}");
    }

    static Box()
    {
      Empty = new Box();
    }
  }
}
