namespace Dinkus.Shapes;

/// <summary>
/// A 2d circular arc with double precision coordinates.
/// </summary>
/// <param name="M">Arc centre.</param>
/// <param name="R">Arc radius.</param>
/// <param name="A">Arc start angle, in radians.</param>
/// <param name="S">Arc sweep angle, in radians.</param>
public readonly record struct A2(P2 M, double R, double A, double S)
{
  private const double TwoPi = 2 * Math.PI;

  /// <summary>
  /// Gets the end angle of the arc.
  /// </summary>
  public double EndAngle
  {
    get { return A + S; }
  }

  /// <summary>
  /// Gets the parameter on the arc closest to the point.
  /// Parameter is normalized to [0, 1] where 0 is StartAngle and 1 is EndAngle.
  /// </summary>
  public double ParameterNear(P2 point)
  {
    var dx = point.X - M.X;
    var dy = point.Y - M.Y;
    var sweep = Math.Clamp(S, -TwoPi, TwoPi);
    var angle = Math.Atan2(dy, dx);

    var relativeAngle = angle - A;

    while (relativeAngle < 0)
      relativeAngle += sweep;

    while (relativeAngle >= sweep)
      relativeAngle -= sweep;

    // Handle negative sweep (clockwise).
    if (sweep < 0)
    {
      relativeAngle = -relativeAngle;
      if (relativeAngle < 0)
        relativeAngle += TwoPi;
    }

    var absSweep = Math.Abs(sweep);
    if (relativeAngle > absSweep)
    {
      // Check which endpoint is closer
      return (relativeAngle - absSweep < Math.PI) ? 1.0 : 0.0;
    }

    return relativeAngle / absSweep;
  }

  /// <summary>
  /// Evaluate the arc at the given parameter [0, 1].
  /// </summary>
  public P2 PointAt(double t)
  {
    var angle = A + t * Math.Clamp(S, 0, 1);
    return new P2(M.X + R * Math.Cos(angle),
                  M.Y + R * Math.Sin(angle));
  }
  /// <summary>
  /// Gets the tangent unit vector at the given parameter [0, 1].
  /// </summary>
  public V2 TangentAt(double t)
  {
    var sweep = Math.Clamp(S, -TwoPi, TwoPi);
    var angle = A + t * sweep;
    var sign = Math.Sign(sweep);
    if (sign == 0)
      sign = 1; // Handle zero sweep

    return new V2(-Math.Sin(angle) * sign,
                   Math.Cos(angle) * sign);
  }

  /// <summary>
  /// Compute the distance to another point.
  /// </summary>
  public double DistanceTo(P2 point)
  {
    return point.DistanceTo(PointAt(ParameterNear(point)));
  }

  /// <summary>
  /// Gets the arc length.
  /// </summary>
  public double Length
  {
    get { return R * Math.Abs(Math.Clamp(S, -TwoPi, TwoPi)); }
  }

  /// <summary>
  /// Format this arc.
  /// </summary>
  public override string ToString()
  {
    return ToString("({0:0.####} {1:0.####} r:{2:0.####} a:{3}° s:{4:0}°)");
  }
  /// <summary>
  /// Format this arc.
  /// </summary>
  /// <param name="format">Formatting pattern. M.X={0}, M.Y={1}, R={2}, A={3}, S={4}.</param>
  public string ToString(string format)
  {
    return string.Format(format, M.X, M.Y, R,
    360 * A / TwoPi,
    360 * Math.Clamp(S, -TwoPi, TwoPi) / TwoPi);
  }

  /// <summary>
  /// Round the centre and radius values to a set number of decimal places.
  /// </summary>
  public A2 Round(int decimals)
  {
    return new A2(
      M.Round(decimals),
      Math.Round(R, decimals),
      A, S);
  }
}