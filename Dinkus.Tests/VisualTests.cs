using OpenColor;
using Dinkus.Shapes;
using System.Drawing;

namespace Dinkus.Tests;

public class VisualTests
{
  /// <summary>
  /// A shader implementation for curve based voronoi shading of the plane.
  /// </summary>
  internal sealed class VoronoiShader
  {
    private ICurveLike A { get; }
    private ICurveLike B { get; }
    public VoronoiShader(ICurveLike a, ICurveLike b)
    {
      A = a;
      B = b;
    }
    public System.Drawing.Color Run(P2 point)
    {
      var dA = A.DistanceTo(point);
      var dB = B.DistanceTo(point);

      static int Level(double smaller, double larger)
      {
        if (smaller / larger >= 0.75) return 1; // Within 25% 
        if (smaller / larger >= 0.50) return 2; // Within 50%
        return 3;                               // Far apart.
      }

      if (dA < dB)
      {
        switch (Level(dA, dB))
        {
          case 1: return OC.Pink1;
          case 2: return OC.Pink2;
          default: return OC.Pink3;
        }
      }
      else
      {
        switch (Level(dB, dA))
        {
          case 1: return OC.Yellow1;
          case 2: return OC.Yellow2;
          default: return OC.Yellow3;
        }
      }
    }
  }

  /// <summary>
  /// A shader implementation for curve based voronoi shading of the plane.
  /// </summary>
  internal sealed class DistanceShader
  {
    private ICurveLike A { get; }
    private double Min { get; }
    private double Max { get; }
    public DistanceShader(ICurveLike a, double min, double max)
    {
      A = a;
      Min = min;
      Max = max;
    }

    public System.Drawing.Color Run(P2 point)
    {
      var d = A.DistanceTo(point);
      int Level(double distance)
      {
        var t = (distance - Min) / (Max - Min);
        return Math.Clamp((int)Math.Ceiling(t * 5), 1, 5);
      }

      switch (Level(d))
      {
        case 1: return OC.Lime5;
        case 2: return OC.Lime4;
        case 3: return OC.Lime3;
        case 4: return OC.Lime2;
        default: return OC.Lime1;
      }
    }
  }

  [Fact]
  public void ViewLineDistanceTo()
  {
    using (var image = new TestImage("LineDistanceTo"))
    {
      var line = new L2(new P2(150, 200), new P2(800, 750));

      var shader = new DistanceShader(line, 0, 500);
      image.RunShader(shader.Run);

      image.DrawLine(line, OC.Lime9);
      image.FillCircle(line.A, OC.Lime9);
      image.FillCircle(line.B, OC.Lime9);
    }
  }

  [Fact]
  public void ViewCircleDistanceTo()
  {
    using (var image = new TestImage("CircleDistanceTo"))
    {
      var circle = new C2(new P2(300, 600), 300);

      var shader = new DistanceShader(circle, 0, 500);
      image.RunShader(shader.Run);

      image.DrawCircle(circle, OC.Lime9);
    }
  }

  [Fact]
  public void ViewArcDistanceTo()
  {
    using (var image = new TestImage("ArcDistanceTo"))
    {
      var arc = new A2(new P2(400, 400), 300, 45, 225);

      var shader = new DistanceShader(arc, 0, 500);
      image.RunShader(shader.Run);

      image.DrawArc(arc, OC.Lime9);
      image.FillCircle(arc.PointAt(0), OC.Lime9);
      image.FillCircle(arc.PointAt(1), OC.Lime9);
    }
  }

  [Fact]
  public void ViewLineParameterNear()
  {
    using (var image = new TestImage("LineParameterNear"))
    {
      var line1 = new L2(new P2(150, 200), new P2(800, 750));
      var line2 = new L2(new P2(200, 700), new P2(700, 100));

      var shader = new VoronoiShader(line1, line2);
      image.RunShader(shader.Run);

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
          image.FillCircle(p, OC.Pink6);
          image.DrawLine(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.FillCircle(p, OC.Orange6);
          image.DrawLine(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.DrawLine(line1, OC.Pink8);
      image.DrawLine(line2, OC.Orange8);
    }
  }

  [Fact]
  public void ViewCircleParameterNear()
  {
    using (var image = new TestImage("CircleParameterNear"))
    {
      var circle1 = new C2(new P2(600, 600), 300);
      var circle2 = new C2(new P2(300, 500), 100);

      var shader = new VoronoiShader(circle1, circle2);
      image.RunShader(shader.Run);

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
          image.FillCircle(p, OC.Pink6);
          image.DrawLine(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.FillCircle(p, OC.Orange6);
          image.DrawLine(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.DrawCircle(circle1, OC.Pink8);
      image.DrawCircle(circle2, OC.Orange8);
    }
  }

  [Fact]
  public void ViewArcParameterNear()
  {
    using (var image = new TestImage("ArcParameterNear"))
    {
      var arc1 = new A2(new P2(600, 600), 300, 0, 200);
      var arc2 = new A2(new P2(400, 500), 200, 180, 270);

      var shader = new VoronoiShader(arc1, arc2);
      image.RunShader(shader.Run);

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
          image.FillCircle(p, OC.Pink6);
          image.DrawLine(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.FillCircle(p, OC.Orange6);
          image.DrawLine(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.DrawArc(arc1, OC.Pink8);
      image.DrawArc(arc2, OC.Orange8);
    }
  }

  [Fact]
  public void ViewArcStartTangent()
  {
    var pUL = new P2(100, 900);
    var pUR = new P2(900, 900);
    var pLL = new P2(100, 100);
    var pLR = new P2(900, 100);

    var tUL = new V2(+1, -1);
    var tUR = new V2(-1, -1);
    var tLL = new V2(+1, +1);
    var tLR = new V2(-1, +1);

    var cUL = OC.Pink8;
    var cUR = OC.Orange8;
    var cLL = OC.Lime8;
    var cLR = OC.Cyan8;

    using (var image = new TestImage("ArcStartTangent"))
    {
      var random = new Random(1);
      for (int i = 0; i < 800; i++)
      {
        var x = random.Next(10, 990);
        var y = random.Next(10, 990);
        var p2 = new P2(x, y);

        P2 p1;
        V2 tn;
        Color cl;

        if (p2.X < 500 && p2.Y < 500)
        {
          p1 = pLL;
          tn = tLL;
          cl = cLL;
        }
        else if (p2.X < 500 && p2.Y >= 500)
        {
          p1 = pUL;
          tn = tUL;
          cl = cUL;
        }
        else if (p2.X >= 500 && p2.Y < 500)
        {
          p1 = pLR;
          tn = tLR;
          cl = cLR;
        }
        else
        {
          p1 = pUR;
          tn = tUR;
          cl = cUR;
        }

        var arc = A2.Create(p1, p2, tn);
        if (arc.S < -270 || arc.S > 270)
          continue;

        image.FillCircle(p2, cl, 2);
        image.DrawArc(arc, cl, 1);
      }

      image.FillCircle(pLL, cLL, 4);
      image.FillCircle(pUL, cUL, 4);
      image.FillCircle(pLR, cLR, 4);
      image.FillCircle(pUR, cUR, 4);
    }
  }
}