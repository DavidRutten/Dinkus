using Dinkus.Shapes;
using OpenColor;

namespace Dinkus.Tests;

public class VisualTests
{
  [Fact]
  public void ViewLineParameterNear()
  {
    using (var image = new TestImage("LineParameterNear"))
    {
      var line1 = new L2(new P2(150, 200), new P2(800, 750));
      var line2 = new L2(new P2(200, 700), new P2(700, 100));

      var random = new Random(1);
      for (int i = 0; i < 800; i++)
      {
        var x = random.Next(10, 990);
        var y = random.Next(10, 990);
        var p = new P2(x, y);

        var t1 = line1.ParameterNear(p);
        var c1 = line1.PointAt(t1);
        var t2 = line2.ParameterNear(p);
        var c2 = line2.PointAt(t2);

        if (p.DistanceTo(c1) < p.DistanceTo(c2))
        {
          image.FillCircle(p, OC.Blue4);
          image.DrawLine(new L2(p, c1), OC.Blue4, 1);
        }
        else
        {
          image.FillCircle(p, OC.Yellow6);
          image.DrawLine(new L2(p, c2), OC.Yellow6, 1);
        }
      }

      image.DrawLine(line1, OC.Blue8);
      image.DrawLine(line2, OC.Yellow9);
    }
  }

  [Fact]
  public void ViewCircleParameterNear()
  {
    using (var image = new TestImage("CircleParameterNear"))
    {
      var circle1 = new C2(new P2(600, 600), 300);
      var circle2 = new C2(new P2(300, 500), 100);

      var random = new Random(1);
      for (int i = 0; i < 800; i++)
      {
        var x = random.Next(10, 990);
        var y = random.Next(10, 990);
        var p = new P2(x, y);

        var t1 = circle1.ParameterNear(p);
        var c1 = circle1.PointAt(t1);
        var t2 = circle2.ParameterNear(p);
        var c2 = circle2.PointAt(t2);

        if (p.DistanceTo(c1) < p.DistanceTo(c2))
        {
          image.FillCircle(p, OC.Blue4);
          image.DrawLine(new L2(p, c1), OC.Blue4, 1);
        }
        else
        {
          image.FillCircle(p, OC.Yellow6);
          image.DrawLine(new L2(p, c2), OC.Yellow6, 1);
        }
      }

      image.DrawCircle(circle1, OC.Blue8);
      image.DrawCircle(circle2, OC.Yellow9);
    }
  }

  [Fact]
  public void ViewArcParameterNear()
  {
    using (var image = new TestImage("ArcParameterNear"))
    {
      var arc1 = new A2(new P2(600, 600), 300, 4, 4);
      var arc2 = new A2(new P2(400, 500), 150, 3, -3);

      var random = new Random(1);
      for (int i = 0; i < 800; i++)
      {
        var x = random.Next(10, 990);
        var y = random.Next(10, 990);
        var p = new P2(x, y);

        var t1 = arc1.ParameterNear(p);
        var c1 = arc1.PointAt(t1);
        var t2 = arc2.ParameterNear(p);
        var c2 = arc2.PointAt(t2);

        if (p.DistanceTo(c1) < p.DistanceTo(c2))
        {
          image.FillCircle(p, OC.Blue4);
          image.DrawLine(new L2(p, c1), OC.Blue4, 1);
        }
        else
        {
          image.FillCircle(p, OC.Yellow6);
          image.DrawLine(new L2(p, c2), OC.Yellow6, 1);
        }
      }

      image.DrawArc(arc1, OC.Blue8);
      image.DrawArc(arc2, OC.Yellow9);
    }
  }
}