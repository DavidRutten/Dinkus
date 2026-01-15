namespace Dinkus.Shapes;

/// <summary>
/// A 2d circular arc with double precision coordinates. The parametrisation of
/// arcs is always in the [0, 1] interval.
/// </summary>
/// <param name="M">Arc centre.</param>
/// <param name="R">Arc radius.</param>
/// <param name="A">Arc start angle, in degrees.</param>
/// <param name="S">Arc sweep angle, in degrees.</param>
public readonly record struct A2(P2 M, double R, double A, double S) : ICurveLike
{
  private const int Full = 360;
  private const double ToDegrees = 180 / Math.PI;
  private const double ToRadians = Math.PI / 180;

  /// <summary>
  /// Gets the end angle of the arc in degrees.
  /// </summary>
  public double E
  {
    get { return A + Math.Clamp(S, -Full, Full); }
  }

  /// <summary>
  /// Gets the parameter on the arc closest to the point.
  /// Parameter is normalized to [0, 1] where 0 is StartAngle and 1 is EndAngle.
  /// </summary>
  public double ParameterNear(P2 point)
  {
    var dx = point.X - M.X;
    var dy = point.Y - M.Y;

    var start = A % Full;
    var sweep = Math.Clamp(S, -Full, Full);
    if (sweep < 0)
    {
      start = E % 360;
      sweep = -sweep;
    }

    var angle = ToDegrees * Math.Atan2(dy, dx);
    var relative = (angle - start) % Full;
    if (relative < 0)
      relative += Full;

    if (relative > sweep)
      return point.DistanceTo(PointAt(0)) < point.DistanceTo(PointAt(1)) ? 0 : 1;

    var t = relative / sweep;

    if (S < 0)
      return 1 - t;
    else
      return t;
  }

  /// <summary>
  /// Evaluate the arc at the given parameter [0, 1].
  /// </summary>
  public P2 PointAt(double t)
  {
    var angle = A + t * Math.Clamp(S, -Full, Full);
    return new P2(M.X + R * Math.Cos(ToRadians * angle),
                  M.Y + R * Math.Sin(ToRadians * angle));
  }
  /// <summary>
  /// Gets the tangent unit vector at the given parameter [0, 1].
  /// </summary>
  public V2 TangentAt(double t)
  {
    var sweep = Math.Clamp(S, -Full, Full);
    var angle = A + t * sweep;
    var sign = Math.Sign(sweep);
    if (sign == 0)
      sign = 1; // Handle zero sweep

    return new V2(-Math.Sin(ToRadians * angle) * sign,
                   Math.Cos(ToRadians * angle) * sign);
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
    get { return R * Math.Abs(Math.Clamp(S, -Full, Full)); }
  }

  /// <summary>
  /// Gets whether this arc is fully closed.
  /// </summary>
  public bool IsClosed
  {
    get { return S <= -Full || S >= Full; }
  }
  /// <summary>
  /// Gets whether this arc is oriented in a clockwise manner.
  /// Positive sweep angles yield an anti-clockwise orientation.
  /// </summary>
  public bool IsClockwise
  {
    get { return S < 0; }
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
    return string.Format(format, M.X, M.Y, R, A, Math.Clamp(S, -Full, Full));
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