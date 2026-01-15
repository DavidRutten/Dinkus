namespace Dinkus.Shapes;

/// <summary>
/// Provides static intersection methods for various shapes.
/// </summary>
public static class X2
{
  /// <summary>
  /// Find the parameters on two infinite lines where they most closely approach each other.
  /// If the lines are parallel, anti-parallel or otherwise degenerate, a (0.5, 0.5) tuple will be returned.
  /// </summary>
  /// <param name="line1">First line segment.</param>
  /// <param name="line2">Second line segment.</param>
  /// <returns>A tuple of line parameters.</returns>
  public static (double parameter1, double parameter2) LineLine(L2 line1, L2 line2)
  {
    var d1 = line1.Span;
    var d2 = line2.Span;
    var delta = line2.A - line1.A;
    var cross = d1.X * d2.Y - d1.Y * d2.X;

    // Handle parallel and degenerate lines.
    if (Math.Abs(cross) < 1e-16)
      return (0.5, 0.5);

    var t1 = (delta.X * d2.Y - delta.Y * d2.X) / cross;
    var t2 = (delta.X * d1.Y - delta.Y * d1.X) / cross;

    return (t1, t2);

    // var d1 = line1.Span;
    // var d2 = line2.Span;
    // var delta = line2.A - line1.A;

    // var dot11 = d1 * d1;
    // var dot22 = d2 * d2;
    // var dot12 = d1 * d2;

    // var denom = dot11 * dot22 - dot12 * dot12;

    // // Handle parallel and degenerate lines.
    // if (Math.Abs(denom) < 1e-16)
    //   return (0.5, 0.5);

    // var t1 = (dot12 * d2 * delta - dot22 * d1 * delta) / denom;
    // var t2 = (dot11 * d2 * delta - dot12 * d1 * delta) / denom;

    // return (t1, t2);
  }

  /// <summary>
  /// Find the parameters on two finite line segments where they most closely approach each other.
  /// If the lines are parallel, anti-parallel or otherwise degenerate, a (0.5, 0.5) tuple will be returned.
  /// </summary>
  /// <param name="line1">First line segment.</param>
  /// <param name="line2">Second line segment.</param>
  /// <returns>A tuple of line parameters.</returns>
  public static (double parameter1, double parameter2) SegmentSegment(L2 line1, L2 line2)
  {
    var (t1, t2) = LineLine(line1, line2);
    if (t1 == 0.5 && t2 == 0.5)
    {
      // For parallel lines, find overlap region.
      var t1_start = line1.ParameterNear(line2.A);
      var t1_end = line1.ParameterNear(line2.B);
      var t2_start = line2.ParameterNear(line1.A);
      var t2_end = line2.ParameterNear(line1.B);

      t1 = 0.5 * (Math.Max(0.0, Math.Min(t1_start, t1_end)) + Math.Min(1.0, Math.Max(t1_start, t1_end)));
      t2 = 0.5 * (Math.Max(0.0, Math.Min(t2_start, t2_end)) + Math.Min(1.0, Math.Max(t2_start, t2_end)));

      return (t1, t2);
    }
    else
    {
      // Clamp and refine for finite segments
      t1 = Math.Clamp(t1, 0.0, 1.0);
      t2 = Math.Clamp(t2, 0.0, 1.0);

      // If either parameter hit a boundary, recompute the other
      if (t1 <= 0.0 || t1 >= 1.0)
        t2 = Math.Clamp(line2.ParameterNear(line1.PointAt(t1)), 0.0, 1.0);

      if (t2 <= 0.0 || t2 >= 1.0)
        t1 = Math.Clamp(line1.ParameterNear(line2.PointAt(t2)), 0.0, 1.0);

      return (t1, t2);
    }
  }

  /// <summary>
  /// Find the intersection points for a line and a circle.
  /// </summary>
  /// <param name="line">Finite line segment.</param>
  /// <param name="circle">Circle.</param>
  /// <returns>Zero, one or two line parameters at intersection points.</returns>
  public static double[] SegmentCircle(L2 line, C2 circle)
  {
    var span = line.Span;
    var radial = line.A - circle.M;

    var a = span * span;
    var b = 2 * (radial * span);
    var c = (radial * radial) - (circle.R * circle.R);

    var discriminant = b * b - 4 * a * c;
    if (discriminant < 0)
      return [];

    discriminant = Math.Sqrt(discriminant);

    var t1 = (-b - discriminant) / (2 * a);
    var t2 = (-b + discriminant) / (2 * a);
    var valid1 = t1 >= 0 && t1 <= 1;
    var valid2 = t2 >= 0 && t2 <= 1;
    if (discriminant < 1e-12)
      valid2 = false;

    if (valid1 && valid2)
      return [t1, t2];
    else if (valid1)
      return [t1];
    else if (valid2)
      return [t2];
    else
      return [];
  }
}