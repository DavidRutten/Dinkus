using System.Drawing;
using OpenColor;
using Dinkus.Shapes;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.ColorSpaces.Conversion;
using Newtonsoft.Json.Serialization;

namespace Dinkus.Tests;

public class VisualTests
{
  /// <summary>
  /// A shader implementation for curve based voronoi shading of the plane.
  /// </summary>
  private sealed class VoronoiShader
  {
    private ICurve A { get; }
    private ICurve B { get; }
    public VoronoiShader(ICurve a, ICurve b)
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
  private sealed class DistanceShader
  {
    private ICurve A { get; }
    private double Min { get; }
    private double Max { get; }
    public DistanceShader(ICurve a, double min, double max)
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

  /// <summary>
  /// Create a curve made up of several lines and arcs.
  /// </summary>
  private static Curve CreateCurve()
  {
    var curve = Curve.Create(new L2(new P2(50, 50), new P2(200, 100)));
    curve = curve.ArcTo(new P2(200, 200));
    curve = curve.ArcTo(new P2(200, 400));
    curve = curve.LineTo(curve.PointAt(1) + 100 * curve.TangentAt(1));
    curve = curve.ArcTo(new P2(500, 700));
    curve = curve.LineTo(curve.PointAt(1) + 100 * curve.TangentAt(1));
    curve = curve.ArcTo(new P2(800, 700));
    curve = curve.LineTo(curve.PointAt(1) + 300 * curve.TangentAt(1));
    curve = curve.ArcTo(new P2(800, 100));

    return curve;
  }

  [Fact]
  public void ViewLineDistanceTo()
  {
    using (var image = new TestImage("LineDistanceTo"))
    {
      var line = new L2(new P2(150, 200), new P2(800, 750));

      var shader = new DistanceShader(line, 0, 500);
      image.Fill(shader.Run);

      image.Draw(line, OC.Lime9);
      image.Fill(line.A, OC.Lime9);
      image.Fill(line.B, OC.Lime9);
    }
  }

  [Fact]
  public void ViewCircleDistanceTo()
  {
    using (var image = new TestImage("CircleDistanceTo"))
    {
      var circle = new C2(new P2(300, 600), 300);

      var shader = new DistanceShader(circle, 0, 500);
      image.Fill(shader.Run);

      image.Draw(circle, OC.Lime9);
    }
  }

  [Fact]
  public void ViewArcDistanceTo()
  {
    using (var image = new TestImage("ArcDistanceTo"))
    {
      var arc = new A2(new P2(400, 400), 300, 45, 225);

      var shader = new DistanceShader(arc, 0, 500);
      image.Fill(shader.Run);

      image.Draw(arc, OC.Lime9);
      image.Fill(arc.PointAt(0), OC.Lime9);
      image.Fill(arc.PointAt(1), OC.Lime9);
    }
  }

  [Fact]
  public void ViewCurveDistanceTo()
  {
    using (var image = new TestImage("CurveDistanceTo"))
    {
      var curve = CreateCurve();

      var shader = new DistanceShader(curve, 0, 200);
      image.Fill(shader.Run);

      for (int t = 1; t < 100; t++)
      {
        var p = curve.PointAt(t / 100.0);
        var v = curve.TangentAt(t / 100.0);
        v = 6 * v.Rotate(90);
        image.Draw(new L2(p, p + v), OC.Lime9, 2);
      }

      image.Draw(curve, OC.Lime9);
      image.Fill(curve.PointAt(0), OC.Lime9, 4);
      image.Fill(curve.PointAt(1), OC.Lime9, 4);
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
      image.Fill(shader.Run);

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
          image.Fill(p, OC.Pink6);
          image.Draw(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.Fill(p, OC.Orange6);
          image.Draw(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.Draw(line1, OC.Pink8);
      image.Draw(line2, OC.Orange8);
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
      image.Fill(shader.Run);

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
          image.Fill(p, OC.Pink6);
          image.Draw(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.Fill(p, OC.Orange6);
          image.Draw(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.Draw(circle1, OC.Pink8);
      image.Draw(circle2, OC.Orange8);
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
      image.Fill(shader.Run);

      var random = new Random(1);
      for (int i = 0; i < 500; i++)
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
          image.Fill(p, OC.Pink6);
          image.Draw(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.Fill(p, OC.Orange6);
          image.Draw(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.Draw(arc1, OC.Pink8);
      image.Draw(arc2, OC.Orange8);
    }
  }

  [Fact]
  public void ViewCurveParameterNear()
  {
    using (var image = new TestImage("CurveParameterNear"))
    {
      var curve1 = CreateCurve();
      var curve2 = Curve.Create(new L2(new P2(50, 950), new P2(250, 700)));
      curve2 = curve2.ArcTo(new P2(350, 100));

      var shader = new VoronoiShader(curve1, curve2);
      image.Fill(shader.Run);

      var random = new Random(1);
      for (int i = 0; i < 800; i++)
      {
        var x = random.Next(10, 990);
        var y = random.Next(10, 990);
        var p = new P2(x, y);

        var t1 = curve1.ParameterNear(p);
        var c1 = curve1.PointAt(t1);
        var t2 = curve2.ParameterNear(p);
        var c2 = curve2.PointAt(t2);

        if (p.DistanceTo(c1) < p.DistanceTo(c2))
        {
          image.Fill(p, OC.Pink6);
          image.Draw(new L2(p, c1), OC.Pink6, 1);
        }
        else
        {
          image.Fill(p, OC.Orange6);
          image.Draw(new L2(p, c2), OC.Orange6, 1);
        }
      }

      image.Draw(curve1, OC.Pink8);
      image.Draw(curve2, OC.Orange8);
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

        image.Fill(p2, cl, 2);
        image.Draw(arc, cl, 1);
      }

      image.Fill(pLL, cLL, 4);
      image.Fill(pUL, cUL, 4);
      image.Fill(pLR, cLR, 4);
      image.Fill(pUR, cUR, 4);
    }
  }

  [Fact]
  public void ViewCircleCircleIntersection()
  {
    using (var image = new TestImage("CircleXCircle"))
    {
      var circles = new C2[9 * 9];
      var random = new Random(2);
      int index = 0;
      for (int i = 100; i < 1000; i += 100)
        for (int j = 100; j < 1000; j += 100)
        {
          var x = i + random.Next(-40, 40);
          var y = j + random.Next(-40, 40);
          var r = random.Next(40, 80);

          var c = new C2(new P2(x, y), r);
          circles[index] = c;
          image.Draw(c, OC.Lime9, 1);

          for (int k = 0; k < index; k++)
          {
            var ts = X2.CircleCircle(c, circles[k]);
            foreach (var t in ts)
              image.Fill(c.PointAt(t), OC.Lime9);
          }

          index++;
        }
    }
  }
}