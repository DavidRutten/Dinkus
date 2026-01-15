using Dinkus.Shapes;

namespace Dinkus.Tests;
public class L2Tests
{
  [Fact]
  public void TestL2Length()
  {
    var a = new L2(P2.Origin, new P2(1, 1));
    Assert.Equal(Math.Sqrt(2), a.Length);
  }
  [Fact]
  public void TestL2Span()
  {
    var a = new L2(new P2(1.1, 2.2), new P2(3.3, 4.4));
    var b = a.Span;
    Assert.Equal(3.3 - 1.1, b.X);
    Assert.Equal(4.4 - 2.2, b.Y);
  }
  [Fact]
  public void TestL2Tangent()
  {
    var a = new L2(new P2(1.1, -2.2), new P2(-3.3, 4.4));
    var b = a.Tangent;
    Assert.Equal(1.0, b.Length);
    Assert.Equal(0.0, a.DistanceTo(a.A + b), 1e-12);
  }

  [Fact]
  public void TestL2PointAt()
  {
    var a = new L2(new P2(-2.2, -3.3), new P2(4.4, 5.5));
    Assert.Equal(0.0, a.PointAt(0.0).DistanceTo(a.A));
    Assert.Equal(0.0, a.PointAt(1.0).DistanceTo(a.B));
    Assert.Equal(0.0, a.PointAt(0.5).DistanceTo(0.5 * a.A + 0.5 * a.B));
  }
  [Fact]
  public void TestL2Near()
  {
    var a = new L2(P2.Origin, new P2(3, 2));
    Assert.Equal(0.0, a.ParameterNear(P2.Origin));
    Assert.Equal(0.0, a.ParameterNear(new P2(-0.2, 0.3)));
    Assert.Equal(0.5, a.ParameterNear(new P2(1.5, 1)));
    Assert.Equal(1.0, a.ParameterNear(new P2(5, 0)));
  }
}