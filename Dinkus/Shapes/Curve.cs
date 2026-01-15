using System.Collections.Immutable;

namespace Dinkus.Shapes;

/// <summary>
/// A sequence of connected curves forming a larger whole.
/// </summary>
public sealed class Curve : ICurveLike
{
  /// <summary>
  /// Hard-coded tolerance used for point coincidence.
  /// </summary>
  public const double Tolerance = 1e-12;
  private readonly ImmutableArray<ICurveLike> _segments;

  #region construction
  /// <summary>
  /// Create a new curve based on a single line.
  /// </summary>
  public static Curve Create(L2 line)
  {
    return new Curve([line]);
  }
  /// <summary>
  /// Create a new curve based on a single circle.
  /// </summary>
  public static Curve Create(C2 circle)
  {
    return new Curve([circle]);
  }
  /// <summary>
  /// Create a new curve based on a single arc.
  /// </summary>
  public static Curve Create(A2 arc)
  {
    return new Curve([arc]);
  }

  private Curve(ImmutableArray<ICurveLike> segments)
  {
    _segments = segments;
    Count = _segments.Length;

    StartPoint = _segments[0].PointAt(0);
    EndPoint = _segments[^1].PointAt(1);
    Closed = StartPoint.DistanceTo(EndPoint) < Tolerance;
  }
  #endregion

  #region properties
  /// <summary>
  /// Gets the start point of this curve.
  /// </summary>
  public P2 StartPoint { get; }
  /// <summary>
  /// Gets the end point of this curve.
  /// </summary>
  public P2 EndPoint { get; }

  /// <summary>
  /// Gets the number of subcurve segments.
  /// </summary>
  public int Count { get; }
  /// <summary>
  /// Gets whether the end-point of the final segment 
  /// matches the start-point of the first segment.
  /// </summary>
  public bool Closed { get; }

  /// <summary>
  /// Gets the length of the entire curve.
  /// </summary>
  public double Length
  {
    get
    {
      var length = 0.0;
      foreach (var s in _segments)
        length += s.Length;
      return length;
    }
  }

  /// <summary>
  /// Gets the segment at the given index.
  /// </summary>
  public ICurveLike this[int index]
  {
    get { return _segments[index]; }
  }

  // BoundingBox
  #endregion;

  #region modification
  /// <summary>
  /// Extend this curve with a line to a point.
  /// If the point already represents the end of this curve,
  /// or if this curve is closed, this method will return 
  /// the curve itself.
  /// </summary>
  public Curve LineTo(P2 point)
  {
    if (Closed || point.DistanceTo(EndPoint) < Tolerance)
      return this;
    return new Curve(_segments.Add(new L2(EndPoint, point)));
  }
  /// <summary>
  /// Extend this curve with a tangent arc to a point.
  /// If the point already represents the end of this curve,
  /// or if this curve is closed, this method will return 
  /// the curve itself.
  /// </summary>
  public Curve ArcTo(P2 point)
  {
    if (Closed || point.DistanceTo(EndPoint) < Tolerance)
      return this;

    return new Curve(_segments.Add(A2.Create(EndPoint, point, _segments[^1].TangentAt(1))));
  }

  /// <summary>
  /// Append a curve to the end of this curve.
  /// If the start of the given curve does not match the end
  /// of this curve, a line segment will be inserted between
  /// the two.
  /// </summary>
  public Curve Append(ICurveLike segment)
  {
    if (Closed || segment.Closed)
      return this;

    var segments = _segments;
    var p1 = PointAt(1);
    var p2 = segment.PointAt(0);

    if (p1.DistanceTo(p2) > Tolerance)
      segments = segments.Add(new L2(p1, p2));

    if (segment is Curve curve)
      segments.AddRange(curve._segments);
    else
      segments = segments.Add(segment);

    return new Curve(segments);
  }
  #endregion

  #region ICurveLike implementation
  /// <summary>
  /// Find the segment index and segment parameter which matches a curve parameter.
  /// </summary>
  /// <param name="t">Curve parameter. Should be in the [0,1] range.</param>
  /// <param name="st">Segment parameter.</param>
  /// <returns>Segment index.</returns>
  private int SegmentIndex(double t, out double st)
  {
    if (t <= 0.0)
    {
      st = 0.0;
      return 0;
    }
    else if (t >= 1.0)
    {
      st = 1.0;
      return Count - 1;
    }
    else
    {
      t *= Count;
      var i = (int)t;
      if (i == Count)
      {
        st = 1.0;
        return Count - 1;
      }
      else
      {
        st = t - i;
        return i;
      }
    }
  }

  /// <summary>
  /// Find the curve parameter represented by a segment index and segment parameter.
  /// </summary>
  /// <param name="t">Segment index.</param>
  /// <param name="st">Segment parameter.</param>
  /// <returns>Curve parameter.</returns>
  private double CurveParameter(int segmentIndex, double segmentParameter)
  {
    var t = (double)segmentIndex / Count;
    t += segmentParameter / Count;
    return Math.Clamp(t, 0, 1);
  }

  /// <summary>
  /// Evaluate the curve at the given parameter. May return invalid points
  /// if the evaluation parameter exceeds the curve domain.
  /// </summary>
  public P2 PointAt(double parameter)
  {
    var i = SegmentIndex(parameter, out var t);
    return _segments[i].PointAt(t);
  }
  /// <summary>
  /// Compute the normalised tangent vector to the curve at the given parameter. 
  /// May return invalid vectors if the evaluation parameter exceeds the curve domain.
  /// </summary>
  public V2 TangentAt(double parameter)
  {
    var i = SegmentIndex(parameter, out var t);
    return _segments[i].TangentAt(t);
  }
  /// <summary>
  /// Compute the distance from a point to the nearest point on the curve.
  /// </summary>
  public double DistanceTo(P2 point)
  {
    var closest = PointAt(ParameterNear(point));
    return point.DistanceTo(closest);
  }
  /// <summary>
  /// Find the parameter on the curve domain which best approximates a point.
  /// </summary>
  /// <param name="point"></param>
  /// <returns></returns>
  public double ParameterNear(P2 point)
  {
    var minD = double.PositiveInfinity;
    var minI = -1;
    var minT = double.NaN;

    for (int i = 0; i < _segments.Length; i++)
    {
      var localT = _segments[i].ParameterNear(point);
      var localD = _segments[i].PointAt(localT).DistanceTo(point);
      if (localD < minD)
      {
        minD = localD;
        minT = localT;
        minI = i;
      }
    }

    if (minI < 0 || double.IsNaN(minT))
      return double.NaN;

    return CurveParameter(minI, minT);
  }
  #endregion
}