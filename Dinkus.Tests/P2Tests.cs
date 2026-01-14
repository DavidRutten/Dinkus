using Dinkus.Shapes;

namespace Dinkus.Tests;

public class P2Tests
{
  [Fact]
  public void TestP2Subtraction()
  {
    var a = new P2(3, 5);
    var b = new P2(11.2, 27.7);
    var c = b - a;

    Assert.Equal(11.2 - 3, c.X);
    Assert.Equal(27.7 - 5, c.Y);
  }
  [Fact]
  public void TestP2Addition()
  {
    var a = new P2(1, 2);
    var b = new V2(5.4, 8.8);
    var c = a + b;

    Assert.Equal(1 + 5.4, c.X);
    Assert.Equal(2 + 8.8, c.Y);
  }
  
  [Fact]
  public void TestP2Distance1()
  {
    var a = P2.Origin + V2.UnitX;
    var b = P2.Origin + V2.UnitY;
    var d = a.DistanceTo(b);
    Assert.Equal(Math.Sqrt(2), d);
  }
  [Fact]
  public void TestP2Distance2()
  {
    var a = P2.Origin + 5 * V2.UnitX;
    var b = P2.Origin + 12 * V2.UnitY;
    var d = a.DistanceTo(b);
    Assert.Equal(13, d);
  }

  [Fact]
  public void TestP2Rounding()
  {
    var a = new P2(Math.PI, -Math.E);
    var b = a.Round(4);
    Assert.Equal(Math.Round(Math.PI, 4), b.X);
    Assert.Equal(Math.Round(-Math.E, 4), b.Y);
  }
}