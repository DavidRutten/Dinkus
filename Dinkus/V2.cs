namespace Dinkus.Shapes;

/// <summary>
/// A 2d vector with double precision coordinates.
/// </summary>
public readonly record struct V2(double X, double Y)
{
  #region arithmetic operators
  /// <summary>
  /// Vector identity.
  /// </summary>
  public static V2 operator +(V2 a)
  {
    return a;
  }
  /// <summary>
  /// Vector negation.
  /// </summary>
  public static V2 operator -(V2 a)
  {
    return new V2(-a.X, -a.Y);
  }
  /// <summary>
  /// Vector addition.
  /// </summary>
  public static V2 operator +(V2 a, V2 b)
  {
    return new V2(a.X + b.X, a.Y + b.Y);
  }
  /// <summary>
  /// Vector subtraction.
  /// </summary>
  public static V2 operator -(V2 a, V2 b)
  {
    return new V2(a.X - b.X, a.Y - b.Y);
  }
  /// <summary>
  /// Vector dot product.
  /// </summary>
  public static double operator *(V2 a, V2 b)
  {
    return a.X * b.X + a.Y * b.Y;
  }
  /// <summary>
  /// Vector scalar product.
  /// </summary>
  public static V2 operator *(V2 vector, double scalar)
  {
    return new V2(vector.X * scalar, vector.Y * scalar);
  }
  /// <summary>
  /// Vector scalar product.
  /// </summary>
  public static V2 operator *(double scalar, V2 vector)
  {
    return new V2(vector.X * scalar, vector.Y * scalar);
  }
  /// <summary>
  /// Vector scalar division.
  /// </summary>
  public static V2 operator /(V2 vector, double scalar)
  {
    return new V2(vector.X / scalar, vector.Y / scalar);
  }
  #endregion

  #region properties
  /// <summary>
  /// Gets the length of this vector.
  /// </summary>
  public double Length
  {
    get { return Math.Sqrt(X * X + Y * Y); }
  }
  /// <summary>
  /// Gets the angle (in radians) of this vector.
  /// </summary>
  public double Angle
  {
    get { return Math.Atan2(Y, X); }
  }

  /// <summary>
  /// Create a unit vector pointing in the same direction.
  /// </summary>
  public V2 Normalise()
  {
    // Zero-length vectors cannot be normalised, 
    // unit-length vectors need not be normalised.
    var length = Length;
    if (Math.Min(Math.Abs(length), Math.Abs(length - 1.0)) < 1e-16)
      return this;

    return new V2(X / length, Y / length);
  }
  /// <summary>
  /// Rotate this vector through an anti-clockwise angle.
  /// </summary>
  /// <param name="angle">Rotation angle, in radians.</param>
  public V2 Rotate(double angle)
  {
    var l = Length;
    var a = Angle + angle;
    return new V2(l * Math.Cos(a),
                  l * Math.Sin(a));
  }

  /// <summary>
  /// Format this vector.
  /// </summary>
  public override string ToString()
  {
    return ToString("[{0:0.####} {1:0.####}]");
  }
  /// <summary>
  /// Format this vector.
  /// </summary>
  /// <param name="format">Formatting pattern. Use {0} and {1} to place X and Y values.</param>
  public string ToString(string format)
  {
    return string.Format(format, X, Y);
  }

  /// <summary>
  /// Round the vector elements to a set number of decimal places.
  /// </summary>
  public V2 Round(int decimals)
  {
    return new V2(
        Math.Round(X, decimals),
        Math.Round(Y, decimals)
    );
  }
  #endregion
}