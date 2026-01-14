namespace Dinkus.Shapes;

/// <summary>
/// A 2d circle with double precision coordinates.
/// </summary>
/// <param name="M">Circle centre or middle.</param>
/// <param name="R">Circle radius.</param>
public readonly record struct C2(P2 M, double R)
{
  /// <summary>
  /// Gets the parameter on the circle closest to the point.
  /// Parameter is in radians, where 0 is at (Center.X + Radius, Center.Y).
  /// </summary>
  public double ParameterNear(P2 point)
  {
    var dx = point.X - M.X;
    var dy = point.Y - M.Y;
    return Math.Atan2(dy, dx);
  }

  /// <summary>
  /// Evaluate the circle at the given parameter (in radians).
  /// </summary>
  public P2 PointAt(double t)
  {
    return new P2(
      M.X + R * Math.Cos(t),
      M.Y + R * Math.Sin(t)
    );
  }

#pragma warning disable CA1822 // Mark members as static
  /// <summary>
  /// Evaluate the circle at the given parameter (in radians).
  /// </summary>
  public V2 TangentAt(double t)
  {
    return new V2(
      -Math.Sin(t),
      +Math.Cos(t)
    );
  }
#pragma warning restore CA1822 // Mark members as static

  /// <summary>
  /// Compute the distance to a point.
  /// </summary>
  public double DistanceTo(P2 point)
  {
    var distToCenter = M.DistanceTo(point);
    return Math.Abs(distToCenter - R);
  }

  /// <summary>
  /// Check if a point is inside or on the circle.
  /// </summary>
  public bool Contains(P2 point)
  {
    return M.DistanceTo(point) <= R;
  }

  /// <summary>
  /// Gets the circumference of the circle.
  /// </summary>
  public double Length
  {
    get { return 2.0 * Math.PI * R; }
  }

  /// <summary>
  /// Format this circle.
  /// </summary>
  public override string ToString()
  {
    return ToString("({0:0.####} {1:0.####}, R:{2:0.####})");
  }
  /// <summary>
  /// Format this circle.
  /// </summary>
  /// <param name="format">Formatting pattern. Center.X={0}, Center.Y={1}, Radius={2}.</param>
  public string ToString(string format)
  {
    return string.Format(format, M.X, M.Y, R);
  }

  /// <summary>
  /// Round center coordinates and radius to a set number of decimal places.
  /// </summary>
  public C2 Round(int decimals)
  {
    return new C2(M.Round(decimals), Math.Round(R, decimals));
  }
}