namespace Dinkus.Shapes;

/// <summary>
/// A 2d point with double precision coordinates.
/// </summary>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
public readonly record struct P2(double X, double Y)
{
  /// <summary>
  /// Gets the origin point.
  /// </summary>
  public static readonly P2 Origin = new(0, 0);

  /// <summary>
  /// Create a vector from b to a.
  /// </summary>
  public static V2 operator -(P2 a, P2 b)
  {
    return new V2(a.X - b.X, a.Y - b.Y);
  }
  /// <summary>
  /// Move a point along a vector.
  /// </summary>
  public static P2 operator +(P2 a, P2 b)
  {
    return new P2(a.X + b.X, a.Y + b.Y);
  }
  /// <summary>
  /// Move a point along a vector.
  /// </summary>
  public static P2 operator +(P2 point, V2 motion)
  {
    return new P2(point.X + motion.X, point.Y + motion.Y);
  }
  /// <summary>
  /// Move a point along a vector.
  /// </summary>
  public static P2 operator -(P2 point, V2 motion)
  {
    return new P2(point.X - motion.X, point.Y - motion.Y);
  }
  /// <summary>
  /// Move a point along a vector.
  /// </summary>
  public static P2 operator +(V2 motion, P2 point)
  {
    return new P2(point.X + motion.X, point.Y + motion.Y);
  }
  /// <summary>
  /// Scale a point based on the origin.
  /// </summary>
  public static P2 operator *(P2 a, double scalar)
  {
    return new P2(a.X * scalar, a.Y * scalar);
  }
  /// <summary>
  /// Scale a point based on the origin.
  /// </summary>
  public static P2 operator *(double scalar, P2 a)
  {
    return new P2(a.X * scalar, a.Y * scalar);
  }

  /// <summary>
  /// Compute the distance to another point.
  /// </summary>
  public double DistanceTo(P2 other)
  {
    var dx = other.X - X;
    var dy = other.Y - Y;
    return Math.Sqrt(dx * dx + dy * dy);
  }

  /// <summary>
  /// Format this coordinate.
  /// </summary>
  public override string ToString()
  {
    return ToString("({0:0.####} {1:0.####})");
  }
  /// <summary>
  /// Format this coordinate.
  /// </summary>
  /// <param name="format">Formatting pattern. Use {0} and {1} to place X and Y values.</param>
  public string ToString(string format)
  {
    return string.Format(format, X, Y);
  }

  /// <summary>
  /// Round the coordinate to a set number of decimal places.
  /// </summary>
  public P2 Round(int decimals)
  {
    return new P2(
        Math.Round(X, decimals),
        Math.Round(Y, decimals)
    );
  }
}