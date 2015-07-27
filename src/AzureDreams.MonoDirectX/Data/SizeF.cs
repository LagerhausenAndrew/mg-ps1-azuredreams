using System;
using Microsoft.Xna.Framework;

namespace AzureDreams.MonoDirectX
{
  /// <summary>
  ///  Stores an ordered pair of integers, typically the width and height of a box.
  /// </summary>
  [Serializable]
  public struct SizeF
  {
    /// <summary>
    /// Initializes an empty size.
    /// </summary>
    public static readonly SizeF Empty;

    private float width;
    private float height;

    /// <summary>
    /// Creates a size from another size (a copy constructor).
    /// </summary>
    /// <param name="size">The size to copy.</param>
    public SizeF(SizeF size)
      : this(size.width, size.height)
    {
    }

    /// <summary>
    /// Creates a size from a point.
    /// </summary>
    /// <param name="pt">The point to copy from.</param>
    public SizeF(Vector2 pt)
      : this(pt.X, pt.Y)
    {
    }

    /// <summary>
    /// Creates a size setting the width and height.
    /// </summary>
    /// <param name="width">The width to give to the size.</param>
    /// <param name="height">The height to give to the size.</param>
    public SizeF(float width, float height)
    {
      this.width = width;
      this.height = height;
    }

    /// <summary>
    /// Adds the widths and heights of two sizes.
    /// </summary>
    public static SizeF operator +(SizeF sz1, SizeF sz2)
    {
      return Add(sz1, sz2);
    }

    /// <summary>
    /// Subtracts the widths and heights of two sizes.
    /// </summary>
    public static SizeF operator -(SizeF sz1, SizeF sz2)
    {
      return Subtract(sz1, sz2);
    }

    /// <summary>
    /// Determines if the widths and heights of the sizes match.
    /// </summary>
    public static bool operator ==(SizeF sz1, SizeF sz2)
    {
      return ((sz1.Width == sz2.Width) && (sz1.Height == sz2.Height));
    }

    /// <summary>
    /// Determines if the widths and heights of the sizes don't match.
    /// </summary>
    public static bool operator !=(SizeF sz1, SizeF sz2)
    {
      return !(sz1 == sz2);
    }

    /// <summary>
    /// Casts a SizeF to Vector2.
    /// </summary>
    public static explicit operator Vector2(SizeF size)
    {
      return new Vector2(size.Width, size.Height);
    }

    /// <summary>
    /// Scales a size by a factor.
    /// </summary>
    public static SizeF operator *(SizeF value, float scaleFactor)
    {
      return Multiply(value, scaleFactor);
    }

    /// <summary>
    /// Scales a size by a factor.
    /// </summary>
    public static SizeF operator *(float scaleFactor, SizeF value)
    {
      return Multiply(value, scaleFactor);
    }

    /// <summary>
    /// Scales a size by a factor.
    /// </summary>
    public static SizeF operator /(SizeF value, float divider)
    {
      return Divide(value, divider);
    }

    /// <summary>
    /// 
    /// </summary>
    public static implicit operator SizeF(Vector2 v)
    {
      return new SizeF(v.X, v.Y);
    }

    /// <summary>
    /// Multiplies the widths and heights of a size by a factor.
    /// </summary>
    /// <param name="value">The size to scale. This will not be modified.</param>
    /// <param name="scaleFactor">The value to multiply the width and height by.</param>
    /// <returns>The product of the size and the scale.</returns>
    public static SizeF Multiply(SizeF value, float scaleFactor)
    {
      SizeF size = new SizeF();
      size.Width = value.Width * scaleFactor;
      size.Height = value.Height * scaleFactor;
      return size;
    }

    /// <summary>
    /// Multiplies the widths and heights of a size by a factor.
    /// </summary>
    /// <param name="value">The size to scale. This will not be modified.</param>
    /// <param name="divider">The value to multiply the width and height by.</param>
    /// <returns>The product of the size and the scale.</returns>
    public static SizeF Divide(SizeF value, float divider)
    {
      float num = 1f / divider;
      return Multiply(value, num);
    }

    /// <summary>
    /// Tests whether the Width or Height property of this RectangleF has a value of zero.
    /// </summary>
    public bool IsEmpty
    {
      get
      {
        if (this.width > 0f)
        {
          return (this.height <= 0f);
        }
        return true;
      }

    }

    /// <summary>
    /// Gets or sets the width of this size.
    /// </summary>
    public float Width
    {
      get { return this.width; }
      set { this.width = value; }
    }

    /// <summary>
    /// Gets or sets the height of this size.
    /// </summary>
    public float Height
    {
      get { return this.height; }
      set { this.height = value; }
    }

    /// <summary>
    /// Adds the widths and heights of two sizes.
    /// </summary>
    /// <param name="sizes">A collection of sizes to add.</param>
    /// <returns>A size containg the added values.</returns>
    public static SizeF Add(params SizeF[] sizes)
    {
      SizeF retval = new SizeF();
      foreach (SizeF size in sizes)
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
    public static SizeF Subtract(params SizeF[] sizes)
    {
      SizeF retval = new SizeF();
      foreach (SizeF size in sizes)
      {
        retval.width -= size.width;
        retval.height -= size.height;
      }
      return retval;
    }

    /// <summary>
    /// Tests to see whether the specified object is a SizeF with the same dimensions as this SizeF.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>true if obj is a SizeF and has the same width and height as this SizeF; otherwise, false.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is SizeF))
      {
        return false;
      }
      SizeF ef = (SizeF)obj;
      return (((ef.Width == this.Width) && (ef.Height == this.Height)) && ef.GetType().Equals(base.GetType()));
    }

    /// <summary>
    /// Returns a hash code for this SizeF structure.
    /// </summary>
    /// <returns>An integer value that specifies a hash value for this SizeF structure.</returns>
    public override int GetHashCode()
    {
      return string.Concat(this.width, ":", this.height).GetHashCode();
    }

    /// <summary>
    /// Converts this size to a Vector2.
    /// </summary>
    /// <returns>A Vector2 containing the same values as this size.</returns>
    public Vector2 ToVector2()
    {
      return (Vector2)this;
    }

    /// <summary>
    /// Converts each component of this size using the Floor function.
    /// </summary>
    /// <returns>The size floored</returns>
    public SizeF Floor()
    {
      return new SizeF((int)Math.Floor(Width), (int)Math.Floor(Height));
    }

    /// <summary>
    /// Truncates this SizeF to fit inside a Size.
    /// </summary>
    /// <returns></returns>
    public Size ToSize()
    {
      return Size.Truncate(this);
    }

    /// <summary>
    /// Creates a human-readable string that represents this SizeF.
    /// </summary>
    /// <returns>A string that represents this Size.</returns>
    public override string ToString()
    {
      return ("{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}");
    }

    static SizeF()
    {
      Empty = new SizeF();
    }
  }
}
