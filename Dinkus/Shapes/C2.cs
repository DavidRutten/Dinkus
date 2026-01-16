namespace Dinkus.Shapes;

/// <summary>
/// A 2d circle with double precision coordinates. The parametrisation of
/// circles is always in the [0, 1) interval.
/// </summary>
/// <param name="M">Circle centre or middle.</param>
/// <param name="R">Circle radius.</param>
public readonly record struct C2(P2 M, double R) : ICurve
{
  private const double TwoPi = 2 * Math.PI;

  /// <summary>
  /// Gets the parameter on the circle closest to the point.
  /// Parameter is in radians, where 0 is at (Center.X + Radius, Center.Y).
  /// </summary>
  /// <returns>The circle parameter closest to the point, in the [0, 1) range.</returns>
  public double ParameterNear(P2 point)
  {
    var dx = point.X - M.X;
    var dy = point.Y - M.Y;
    var t = Math.Atan2(dy, dx) / TwoPi;
    if (t < 0)
      t += 1;
    return t;
  }

  /// <summary>
  /// Evaluate the circle at the given parameter (in radians).
  /// </summary>
  /// <param name="t">Circle parameter in the [0, 1) range.</param>
  public P2 PointAt(double t)
  {
    // Floating point inaccuracies occur at 'nice' 
    // parameters, so let's handle them separately.
    switch (t)
    {
      case 0.0:
      case 1.0:
        return new P2(M.X + R, M.Y);
      case 0.25:
        return new P2(M.X, M.Y + R);
      case 0.5:
        return new P2(M.X - R, M.Y);
      case 0.75:
        return new P2(M.X, M.Y - R);
      default:
        return new P2(M.X + R * Math.Cos(TwoPi * t),
                      M.Y + R * Math.Sin(TwoPi * t));
    }
  }

  /// <summary>
  /// Evaluate the circle at the given parameter (in radians).
  /// </summary>
  public V2 TangentAt(double t)
  {
    // Floating point inaccuracies occur at 'nice' 
    // parameters, so let's handle them separately.
    switch (t)
    {
      case 0.0:
      case 1.0:
        return V2.UnitY;
      case 0.25:
        return -V2.UnitX;
      case 0.5:
        return -V2.UnitY;
      case 0.75:
        return V2.UnitX;
      default:
        return new V2(-Math.Sin(TwoPi * t),
                      +Math.Cos(TwoPi * t));
    }
  }

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
  /// Gets whether this curve is closed.
  /// </summary>
  public bool Closed
  {
    get { return true; }
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