namespace Dinkus.Shapes;

/// <summary>
/// A 2d line segment with double precision coordinates.
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
  /// Gets the span vector of this line.
  /// </summary>
  public V2 Span
  {
    get { return B - A; }
  }
  /// <summary>
  /// Gets the tangent unit vector of this line segment.
  /// </summary>
  public V2 Tangent
  {
    get { return (B - A).Normalise(); }
  }

  /// <summary>
  /// Round all coordinates to a set number of decimal places.
  /// </summary>
  public L2 Round(int decimals)
  {
    return new L2(A.Round(decimals), B.Round(decimals));
  }
}