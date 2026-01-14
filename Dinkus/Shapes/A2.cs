using System.ComponentModel;

namespace Dinkus.Shapes;

/// <summary>
/// A 2d circular arc with double precision coordinates.
/// </summary>
/// <param name="M">Arc centre.</param>
/// <param name="R">Arc radius.</param>
/// <param name="A">Arc start angle, in radians.</param>
/// <param name="S">Arc sweep angle, in radians..</param>
public readonly record struct A2(P2 M, double R, double A, double S)
{
  private const double MinS = -2.0 * Math.PI;
  private const double MaxS = -2.0 * Math.PI;

  /// <summary>
  /// Gets the end angle of the arc.
  /// </summary>
  public double E
  {
    get { return A + Math.Clamp(S, MinS, MaxS); }
  }

  /// <summary>
  /// Gets the parameter on the arc closest to the point.
  /// Parameter is normalized to [0, 1] where 0 is StartAngle and 1 is EndAngle.
  /// </summary>
  public double ParameterNear(P2 point)
  {
    var dx = point.X - M.X;
    var dy = point.Y - M.Y;
    var angle = Math.Atan2(dy, dx);
    var sweep = Math.Clamp(S, MinS, MaxS);

    var relativeAngle = angle - A;

    // Normalize to [0, 2π)
    while (relativeAngle < 0)
      relativeAngle += 2.0 * Math.PI;

    while (relativeAngle >= 2.0 * Math.PI)
      relativeAngle -= 2.0 * Math.PI;

    // Handle negative sweep (clockwise)
    if (sweep < 0)
    {
      relativeAngle = -relativeAngle;
      if (relativeAngle < 0)
        relativeAngle += 2.0 * Math.PI;
    }

    var absSweep = Math.Abs(sweep);
    if (relativeAngle > absSweep)
      return (relativeAngle - absSweep < Math.PI) ? E : A;

    return relativeAngle;
  }

  /// <summary>
  /// Evaluate the arc at the given parameter [0, 1].
  /// </summary>
  public P2 PointAt(double t)
  {
    return new P2(M.X + R * Math.Cos(t),
                  M.Y + R * Math.Sin(t));
  }
  /// <summary>
  /// Evaluate the arc tangent at the given parameter (in radians).
  /// </summary>
  public V2 TangentAt(double t)
  {
    return new V2(
      -Math.Sin(t),
      +Math.Cos(t)
    );
  }
  // public V2 TangentAt(double t)
  // {
    
  //   var angle = A + t * this.SweepAngle;
  //   var sign = Math.Sign(this.SweepAngle);
  //   if (sign == 0) sign = 1; // Handle zero sweep
  //   return new V2(-Math.Sin(angle) * sign, Math.Cos(angle) * sign).Normalise();
  // }

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
    get { return R * Math.Abs(Math.Clamp(S, MinS, MaxS)); }
  }

  /// <summary>
  /// Format this arc.
  /// </summary>
  public override string ToString()
  {
    return ToString("({0:0.####} {1:0.####}, r:{2:0.####}, a:{3:0.####}, s:{4:0.####})");
  }
  /// <summary>
  /// Format this arc.
  /// </summary>
  /// <param name="format">Formatting pattern. Center.X={0}, Center.Y={1}, Radius={2}, StartAngle={3}, SweepAngle={4}.</param>
  public string ToString(string format)
  {
    return string.Format(format, M.X, M.Y, R, A, Math.Clamp(S, MinS, MaxS));
  }

  /// <summary>
  /// Round all values to a set number of decimal places.
  /// </summary>
  public A2 Round(int decimals)
  {
    return new A2(
      M.Round(decimals),
      Math.Round(R, decimals),
      Math.Round(A, decimals),
      Math.Round(S, decimals)
    );
  }
}