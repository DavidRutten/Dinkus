namespace Dinkus.Shapes;

/// <summary>
/// A 2d point with double precision coordinates.
/// </summary>
/// <param name="X">X coordinate.</param>
/// <param name="Y">Y coordinate.</param>
public readonly record struct P2(double X, double Y)
{
  /// <summary>
  /// Compute the distance to another point.
  /// </summary>
  public double DistanceTo(P2 other)
  {
    var dx = other.X - X;
    var dy = other.Y - Y;
    return Math.Sqrt(dx * dx + dy * dy);
  }
  // /// <summary>
  // /// Compute the quadrance (i.e. the distance-squared) to another point.
  // /// </summary>
  // public double QuadranceTo(P2 other)
  // {
  //   var dx = other.X - X;
  //   var dy = other.Y - Y;
  //   return dx * dx + dy * dy;
  // }

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

/// <summary>
/// A 2d line segment with double precisio coordinates.
/// </summary>
/// <param name="A">Line start point.</param>
/// <param name="B">Line end point.</param>
public readonly record struct L2(P2 A, P2 B)
{
  /// <summary>
  /// Gets the parameter on the line closest to the point.
  /// </summary>
  public double ParameterNear(P2 point)
  {
    var dx = B.X - A.X;
    var dy = B.Y - A.Y;

    var px = point.X - A.X;
    var py = point.Y - A.Y;

    var quadrance = dx * dx + dy * dy;
    if (quadrance == 0.0)
      return 0.0;

    var t = (px * dx + py * dy) / quadrance;
    return Math.Clamp(t, 0.0, 1.0);
  }

  /// <summary>
  /// Evaluate the line at the given parameter.
  /// </summary>
  public P2 PointAt(double t)
  {
    return new P2(
      (1 - t) * A.X + t * B.X,
      (1 - t) * A.Y + t * B.Y
    );
  }

  /// <summary>
  /// Compute the distance to another point.
  /// </summary>
  public double DistanceTo(P2 point)
  {
    return point.DistanceTo(PointAt(ParameterNear(point)));
  }

  /// <summary>
  /// Find the parameters on two line segments where they most closely approach each other.
  /// </summary>
  /// <param name="line1">First line segment.</param>
  /// <param name="line2">Second line segment.</param>
  /// <param name="t1">Parameter on line1 interior.</param>
  /// <param name="t2">Parameter on line2 interior.</param>
  /// <returns>True if a closest approach was found, false if lines are degenerate.</returns>
  public static bool Intersect(L2 line1, L2 line2, out double t1, out double t2)
  {
    var d1x = line1.B.X - line1.A.X;
    var d1y = line1.B.Y - line1.A.Y;
    var d2x = line2.B.X - line2.A.X;
    var d2y = line2.B.Y - line2.A.Y;

    var dx = line2.A.X - line1.A.X;
    var dy = line2.A.Y - line1.A.Y;

    var d1_dot_d1 = d1x * d1x + d1y * d1y;
    var d2_dot_d2 = d2x * d2x + d2y * d2y;
    var d1_dot_d2 = d1x * d2x + d1y * d2y;

    var denom = d1_dot_d1 * d2_dot_d2 - d1_dot_d2 * d1_dot_d2;

    // Check for degenerate or parallel lines.
    if (Math.Abs(denom) < 1e-16)
    {
      // Lines are parallel or degenerate
      // Use closest endpoint approach
      t1 = 0.0;
      t2 = line1.A.DistanceTo(line2.A) < line1.A.DistanceTo(line2.B) ? 0.0 : 1.0;
      return d1_dot_d1 > 1e-10 && d2_dot_d2 > 1e-10;
    }

    var d1_dot_delta = d1x * dx + d1y * dy;
    var d2_dot_delta = d2x * dx + d2y * dy;

    // Compute unclamped parameters
    t1 = (d1_dot_d2 * d2_dot_delta - d2_dot_d2 * d1_dot_delta) / denom;
    t2 = (d1_dot_d1 * d2_dot_delta - d1_dot_d2 * d1_dot_delta) / denom;

    // Clamp to finite segments and handle edge cases
    t1 = Math.Clamp(t1, 0.0, 1.0);
    t2 = Math.Clamp(t2, 0.0, 1.0);

    // If one parameter was clamped, recompute the other
    if (t1 == 0.0 || t1 == 1.0)
      t2 = Math.Clamp(line2.ParameterNear(line1.PointAt(t1)), 0.0, 1.0);

    if (t2 == 0.0 || t2 == 1.0)
      t1 = Math.Clamp(line1.ParameterNear(line2.PointAt(t2)), 0.0, 1.0);

    return true;
  }

  /// <summary>
  /// Format this line.
  /// </summary>
  public override string ToString()
  {
    return ToString("({0:0.####} {1:0.####}, {0:0.####} {1:0.####})");
  }
  /// <summary>
  /// Format this line.
  /// </summary>
  /// <param name="format">Formatting pattern.A.X={0}, A.Y={1}, B.X={2}, B.Y={3}.</param>
  public string ToString(string format)
  {
    return string.Format(format, A.X, A.Y, B.X, B.Y);
  }

  /// <summary>
  /// Gets the length of the line.
  /// </summary>
  public double Length
  {
    get { return A.DistanceTo(B); }
  }

  /// <summary>
  /// Round all coordinates to a set number of decimal places.
  /// </summary>
  public L2 Round(int decimals)
  {
    return new L2(A.Round(decimals), B.Round(decimals));
  }
}