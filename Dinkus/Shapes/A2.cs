namespace Dinkus.Shapes;

/// <summary>
/// A 2d circular arc with double precision coordinates. The parametrisation of
/// arcs is always in the [0, 1] interval.
/// </summary>
/// <param name="M">Arc centre.</param>
/// <param name="R">Arc radius.</param>
/// <param name="A">Arc start angle, in degrees.</param>
/// <param name="S">Arc sweep angle, in degrees.</param>
public readonly record struct A2(P2 M, double R, double A, double S) : ICurve
{
  private const int Full = 360;
  internal const double ToDegrees = 180 / Math.PI;
  internal const double ToRadians = Math.PI / 180;

  /// <summary>
  /// Creates an arc from start point, end point, and start tangent direction.
  /// </summary>
  /// <param name="start">Arc start point.</param>
  /// <param name="end">Arc end point.</param>
  /// <param name="tangent">Tangent direction at start point (will be normalized).</param>
  /// <returns>A valid arc, or an arc with zero sweep angle if the geometry is degenerate.</returns>
  public static A2 Create(P2 start, P2 end, V2 tangent)
  {
    var chord = new L2(start, end);

    // Zero length arc.
    if (chord.Length < 1e-12)
      return new A2(start, 0, 0, 0);

    tangent = tangent.Normalise();

    // Flat arc.
    if ((start + tangent).DistanceTo(start + chord.Tangent) < 1e-8)
      return new A2(start, 0, 0, 0);

    var perp1 = new L2(start, start + tangent.Rotate(90));
    var mid = chord.PointAt(0.5);
    var perp2 = new L2(mid, mid + chord.Tangent.Rotate(90));

    var (t1, t2) = X2.LineLine(perp1, perp2);
    if (t1 == 0.5 && t2 == 0.5)
      return new A2(start, 0, 0, 0);

    var centre = 0.5 * perp1.PointAt(t1) +
                 0.5 * perp2.PointAt(t2);

    var angle1 = ToDegrees * Math.Atan2(start.Y - centre.Y, start.X - centre.X);
    var angle2 = ToDegrees * Math.Atan2(end.Y - centre.Y, end.X - centre.X);

    var sweep = angle2 - angle1;
    while (sweep > 180) sweep -= Full;
    while (sweep < -180) sweep += Full;

    var radial = (start - centre).Normalise();
    var cross = radial.X * tangent.Y - radial.Y * tangent.X;

    if ((cross > 0 && sweep < 0) || (cross < 0 && sweep > 0))
      sweep += sweep > 0 ? -Full : Full;

    return new A2(centre, centre.DistanceTo(start), angle1, sweep);
  }

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
  /// Gets whether this curve is closed.
  /// </summary>
  public bool Closed
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