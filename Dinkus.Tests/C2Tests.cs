using Dinkus.Shapes;

namespace Dinkus.Tests;

public class C2Tests
{
  [Fact]
  public void TestC2Length()
  {
    var a = new C2(P2.Origin, 1);
    Assert.Equal(2 * Math.PI, a.Length);
  }

  [Fact]
  public void TestC2Near()
  {
    var a = new C2(P2.Origin, 1);
    var b = a.ParameterNear(new P2(1, 1));
    Assert.Equal(0.125, b);

    var c = a.ParameterNear(new P2(-0.1, -0.1));
    Assert.Equal(5.0 / 8.0, c);
  }

  [Fact]
  public void TestC2Distance()
  {
    var a = new C2(P2.Origin, 5);
    var b = a.DistanceTo(new P2(10, 0));
    Assert.Equal(5, b);

    var c = a.DistanceTo(new P2(10, 10));
    Assert.Equal(Math.Sqrt(200) - 5, c);
  }

  [Fact]
  public void TestC2PointAt()
  {
    var a = new C2(new P2(1, 1), 1);
    var b = a.PointAt(0.125);
    Assert.Equal(1 + 0.5 * Math.Sqrt(2), b.X);
    Assert.Equal(b.X, b.Y);

    var c = a.PointAt(0.5);
    Assert.Equal(0, c.X);
    Assert.Equal(1, c.Y);
  }

  [Fact]
  public void TestC2TangentAt()
  {
    var a = new C2(P2.Origin, 1);
    var b = a.TangentAt(0.1);
    Assert.Equal(1, b.Length);
    Assert.Equal((0.2 + 0.5) * Math.PI, b.Angle);

    var c = a.TangentAt(0.5);
    Assert.Equal(0, c.X);
    Assert.Equal(-1, c.Y);
  }
}