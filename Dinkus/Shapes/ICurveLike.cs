namespace Dinkus.Shapes;

/// <summary>
/// Interface for all curve like types.
/// </summary>
public interface ICurveLike
{
  /// <summary>
  /// Gets the length of this curve.
  /// </summary>
  double Length { get; }

  /// <summary>
  /// Evaluate the curve at the given parameter. May return invalid points
  /// if the evaluation parameter exceeds the curve domain.
  /// </summary>
  P2 PointAt(double parameter);
  /// <summary>
  /// Compute the normalised tangent vector to the curve at the given parameter. 
  /// May return invalid vectors if the evaluation parameter exceeds the curve domain.
  /// </summary>
  V2 TangentAt(double parameter);
  /// <summary>
  /// Compute the distance from a point to the nearest point on the curve.
  /// </summary>
  double DistanceTo(P2 point);
  /// <summary>
  /// Find the parameter on the curve domain which best approximates a point.
  /// </summary>
  /// <param name="point"></param>
  /// <returns></returns>
  double ParameterNear(P2 point);
}
