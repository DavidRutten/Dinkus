using Dinkus.Shapes;

namespace Dinkus.Tests;

public class V2Tests
{
  [Fact]
  public void TestV2Subtraction()
  {
    var a = new V2(3, 5);
    var b = new V2(11.2, 27.7);
    var c = b - a;

    Assert.Equal(11.2 - 3, c.X);
    Assert.Equal(27.7 - 5, c.Y);
  }
  [Fact]
  public void TestV2Addition()
  {
    var a = new V2(1.1, 2.2);
    var b = new V2(3.3, 4.4);
    var c = a + b;

    Assert.Equal(1.1 + 3.3, c.X);
    Assert.Equal(2.2 + 4.4, c.Y);
  }
  [Fact]
  public void TestV2ScalarProduct()
  {
    var a = new V2(2.2, 3.3);
    var c = a * 4.6;
    Assert.Equal(2.2 * 4.6, c.X);
    Assert.Equal(3.3 * 4.6, c.Y);
  }

  [Fact]
  public void TestV2InnerProduct()
  {
    var a = new V2(1, 0);
    var b = new V2(2.4, 3.7);
    var c = a * b;
    Assert.Equal(2.4, c);
  }
  [Fact]
  public void TestV2PolarConstruction()
  {
    var a = V2.FromAngle(0.0);
    Assert.Equal(1, a.X, 1e-12);
    Assert.Equal(0, a.Y, 1e-12);

    var b = V2.FromAngle(Math.PI / 2);
    Assert.Equal(0, b.X, 1e-12);
    Assert.Equal(1, b.Y, 1e-12);

    var c = V2.FromAngle(Math.PI / 4);
    Assert.Equal(Math.Sqrt(2) / 2, c.X, 1e-12);
    Assert.Equal(Math.Sqrt(2) / 2, c.Y, 1e-12);
  }

  [Fact]
  public void TestV2Length()
  {
    var a = new V2(-6.9, 5.7);
    Assert.Equal(Math.Sqrt(Math.Pow(-6.9, 2) + Math.Pow(5.7, 2)), a.Length);

    a = new V2(12, 5);
    Assert.Equal(13, a.Length);
  }
  [Fact]
  public void TestV2Angle()
  {
    var a = new V2(3, 2);
    Assert.Equal(Math.Atan(2.0 / 3.0), a.Angle);
  }
  [Fact]
  public void TestV2Normalise()
  {
    var a = new V2(7.7, 9.9);
    var b = a.Normalise();
    Assert.Equal(1.0, b.Length);
    Assert.Equal(a.Angle, b.Angle);
  }
  [Fact]
  public void TestV2Rotate()
  {
    var a = new V2(7.7, 9.9);
    var b = a.Rotate(0.2);
    Assert.Equal(a.Length, b.Length);
    Assert.Equal(a.Angle + 0.2, b.Angle);
  }
}