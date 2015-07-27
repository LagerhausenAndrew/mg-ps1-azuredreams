using System;
using Microsoft.Xna.Framework;

namespace AzureDreams.MonoDirectX
{
  /// <summary>
  /// Stores an ordered pair of integers, typically the width and height of a box.
  /// </summary>
  [Serializable]
  public struct Size
  {
    /// <summary>
    /// Initializes an empty size.
    /// </summary>
    public static readonly Size Empty;

    private int width;
    private int height;

    /// <summary>
    /// Creates a size from a point.
    /// </summary>
    /// <param name="pt">The point to pull size data from.</param>
    public Size(Point pt)
      : this(pt.X, pt.Y)
    {

    }

    /// <summary>
    /// Creates a size from the width and height.
    /// </summary>
    /// <param name="width">The width to assign.</param>
    /// <param name="height">The height to assign.</param>
    public Size(int width, int height)
    {
      this.width = width;
      this.height = height;
    }

    /// <summary>
    /// Casts a Size to a SizeF.
    /// </summary>
    public static implicit operator SizeF(Size p)
    {
      return new SizeF((float)p.Width, (float)p.Height);
    }

    /// <summary>
    /// Adds the widths and heights of two sizes.
    /// </summary>
    public static Size operator +(Size sz1, Size sz2)
    {
      return Add(sz1, sz2);
    }

    /// <summary>
    /// Subtracts the widths and heights of two sizes.
    /// </summary>
    public static Size operator -(Size sz1, Size sz2)
    {
      return Subtract(sz1, sz2);
    }

    /// <summary>
    /// Determines if the widths and heights of the sizes match.
    /// </summary>
    public static bool operator ==(Size sz1, Size sz2)
    {
      return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
    }

    /// <summary>
    /// Determines if the widths and heights of the sizes don't match.
    /// </summary>
    public static bool operator !=(Size sz1, Size sz2)
    {
      return !(sz1 == sz2);
    }

    /// <summary>
    /// Casts a Size to Point.
    /// </summary>
    public static explicit operator Point(Size size)
    {
      return new Point(size.Width, size.Height);
    }

    /// <summary>
    /// Gets a value indicating if this has a width and height of 0.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        return ((this.width == 0) && (this.height == 0));
      }
    }

    /// <summary>
    /// Gets or sets the width of this size.
    /// </summary>
    public int Width
    {
      get { return this.width; }
      set { this.width = value; }
    }

    /// <summary>
    /// Gets or sets the height of this size.
    /// </summary>
    public int Height
    {
      get { return this.height; }
      set { this.height = value; }
    }

    /// <summary>
    /// Adds the widths and heights of two sizes.
    /// </summary>
    /// <param name="sizes">A collection of sizes to add.</param>
    /// <returns>A size containg the added values.</returns>
    public static Size Add(params Size[] sizes)
    {
      Size retval = new Size();
      foreach (Size size in sizes)
      {
        retval.width += size.width;
        retval.height += size.height;
      }
      return retval;
    }

    /// <summary>
    /// Subtracts the widths and heights of two sizes.
    /// </summary>
    /// <param name="sizes">A collection of sizes to subtract.</param>
    /// <returns>A size containg the subtracted values.</returns>
    public static Size Subtract(params Size[] sizes)
    {
      Size retval = new Size();
      foreach (Size size in sizes)
      {
        retval.width -= size.width;
        retval.height -= size.height;
      }
      return retval;
    }

    /// <summary>
    /// Converts the specified SizeF structure to a Size structure by rounding the values of the Size structure to the 
    /// next higher integer values.
    /// </summary>
    /// <param name="value">The SizeF structure to convert.</param>
    /// <returns>The converted SizeF value.</returns>
    public static Size Ceiling(SizeF value)
    {
      return new Size((int)Math.Ceiling((double)value.Width), (int)Math.Ceiling((double)value.Height));
    }

    /// <summary>
    /// Converts the specified SizeF structure to a Size structure by truncating the values of the SizeF structure to the 
    /// next lower integer values.
    /// </summary>
    /// <param name="value">The SizeF structure to convert.</param>
    /// <returns>The converted SizeF value.</returns>
    public static Size Truncate(SizeF value)
    {
      return new Size((int)value.Width, (int)value.Height);
    }

    /// <summary>
    /// Converts the specified SizeF structure to a Size structure by rounding the values of the SizeF structure to the 
    /// nearest integer values.
    /// </summary>
    /// <param name="value">The SizeF structure to convert.</param>
    /// <returns>The converted SizeF value.</returns>
    public static Size Round(SizeF value)
    {
      return new Size((int)Math.Round((double)value.Width), (int)Math.Round((double)value.Height));
    }

    /// <summary>
    /// Tests to see whether the specified object is a Size with the same dimensions as this Size.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>true if obj is a Size and has the same width and height as this Size; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is Size))
      {
        return false;
      }
      Size size = (Size)obj;
      return ((size.width == this.width) && (size.height == this.height));
    }

    /// <summary>
    /// Returns a hash code for this Size structure.
    /// </summary>
    /// <returns>An integer value that specifies a hash value for this Size structure.</returns>
    public override int GetHashCode()
    {
      return (this.width ^ this.height);
    }

    /// <summary>
    /// Creates a human-readable string that represents this Size.
    /// </summary>
    /// <returns>A string that represents this Size.</returns>
    public override string ToString()
    {
      return ("{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}");
    }

    static Size()
    {
      Empty = new Size();
    }
  }
}
