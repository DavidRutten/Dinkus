using Dinkus.Shapes;

namespace Dinkus.Tests;
public class A2Tests
{
  [Fact]
  public void TestA2EndAngle()
  {
    var a = new A2(P2.Origin, 1, 0.4, 0.9);
    Assert.Equal(1.3, a.E);

    var b = new A2(P2.Origin, 1, 0.4, 100);
    Assert.Equal(0.4 + 2 * Math.PI, b.E);
  }

  [Fact]
  public void TestA2PointAt()
  {
    var a = new A2(P2.Origin, 2, 0.25 * Math.PI, Math.PI);
    var b = a.PointAt(0);
    Assert.Equal(Math.Sqrt(2), b.X);
    Assert.Equal(b.X, b.Y, 1e-12);

    var c = a.PointAt(0.25);
    Assert.Equal(0, c.X, 1e-12);
    Assert.Equal(2, c.Y, 1e-12);

    var d = a.PointAt(1.0);
    Assert.Equal(-Math.Sqrt(2), d.X, 1e-12);
    Assert.Equal(d.X, d.Y, 1e-12);
  }

  [Fact]
  public void TestA2Length()
  {
    var a = new A2(P2.Origin, 1, 0, Math.PI);
    Assert.Equal(Math.PI, a.Length);

    var b = new A2(P2.Origin, 10, 0, 1.5 * Math.PI);
    Assert.Equal(0.75 * (20 * Math.PI), b.Length);
  }

  [Fact]
  public void TestA2Near1()
  {
    var a = new A2(P2.Origin, 1, 0, 1.5 * Math.PI);
    var b = a.ParameterNear(new P2(2, 0));
    Assert.Equal(0, b);

    var c = a.ParameterNear(new P2(0, -2));
    Assert.Equal(1, c);
  }

  [Fact]
  public void TestA2Near2()
  {
    var a = new A2(P2.Origin, 1, 0, 1.5 * Math.PI);
    var b = a.ParameterNear(new P2(2, -0.5));
    Assert.Equal(0, b);

    var c = a.ParameterNear(new P2(0.5, -2));
    Assert.Equal(1, c);
  }
}