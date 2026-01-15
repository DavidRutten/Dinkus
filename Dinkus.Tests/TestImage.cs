using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;

using Dinkus.Shapes;

namespace Dinkus.Tests;

/// <summary>
/// An image for drawing unit test results.
/// </summary>
public sealed class TestImage : IDisposable
{
  private const int Width = 1000;
  private const int Height = 1000;

  private readonly Image<Rgba32> Image;
  public string Name { get; }
  public string Folder { get; }

  public TestImage(string name)
  {
    if (string.IsNullOrWhiteSpace(name))
      throw new ArgumentException($"{nameof(name)} must be a valid file name.");

    Name = name;

    var folder = new DirectoryInfo(AppContext.BaseDirectory);
    while (true)
    {
      var path = folder.FullName + System.IO.Path.DirectorySeparatorChar;
      if (File.Exists(path + "Dinkus.Tests.csproj"))
      {
        Folder = path + System.IO.Path.DirectorySeparatorChar + "img";
        break;
      }

      folder = folder.Parent;
      if (folder is null)
        throw new DirectoryNotFoundException("The dinkus test project directory could not be found.");
    }

    Image = new Image<Rgba32>(Width, Height);
    Image.Mutate(ctx => ctx.Fill(GdiToSix(OpenColor.OC.Gray0)));
  }
  private bool _disposed = false;
  public void Dispose()
  {
    if (!_disposed)
    {
      _disposed = true;
      Directory.CreateDirectory(Folder);

      var path = System.IO.Path.Combine(Folder, Name + ".png");
      Image?.Save(path);
      Image?.Dispose();
    }
  }

  private static Color GdiToSix(System.Drawing.Color colour)
  {
    var argb = new Argb32(colour.R, colour.G, colour.B);
    return new Color(argb);
  }

  public void DrawLine(L2 line, System.Drawing.Color colour, float width = 4f)
  {
    var p1 = new PointF((float)line.A.X, (float)line.A.Y);
    var p2 = new PointF((float)line.B.X, (float)line.B.Y);
    Image.Mutate(ctx => { ctx.DrawLine(GdiToSix(colour), width, p1, p2); });
  }
  public void DrawCircle(C2 circle, System.Drawing.Color colour, float width = 4f)
  {
    var path = new EllipsePolygon((float)circle.M.X, (float)circle.M.Y, (float)circle.R);
    Image.Mutate(ctx => { ctx.Draw(GdiToSix(colour), width, path); });
  }
  public void DrawArc(A2 arc, System.Drawing.Color colour, float width = 4f)
  {
    var centre = new PointF((float)arc.M.X, (float)arc.M.Y);
    var radius = (float)arc.R;
    var angle = (float)arc.A;
    var sweep = (float)arc.S;

    var path = new PathBuilder().AddArc(centre, radius, radius, 0, angle, sweep).Build();

    Image.Mutate(ctx => ctx.Draw(GdiToSix(colour), width, path));
  }

  public void FillCircle(P2 point, System.Drawing.Color colour, float radius = 3)
  {
    var circle = new EllipsePolygon((float)point.X, (float)point.Y, radius);
    Image.Mutate(ctx => ctx.Fill(GdiToSix(colour), circle));
  }

  /// <summary>
  /// Runs a "shader" function across the image.
  /// </summary>
  /// <param name="shader">Function taking (x,y) pixel coordinates, returning a Color.</param>
  /// <param name="accuracy">1 = per-pixel, >1 = block sampling.</param>
  public void RunShader(Func<P2, System.Drawing.Color> shader, int accuracy = 2)
  {
    ArgumentNullException.ThrowIfNull(shader);
    accuracy = Math.Clamp(accuracy, 1, 100);

    for (int y = 0; y < Height; y += accuracy)
      for (int x = 0; x < Width; x += accuracy)
      {
        var sampleX = x + accuracy / 2f;
        var sampleY = y + accuracy / 2f;

        var colour = GdiToSix(shader(new P2(sampleX, sampleY)));
        int blockWidth = Math.Min(accuracy, Width - x);
        int blockHeight = Math.Min(accuracy, Height - y);

        for (int j = 0; j < blockHeight; j++)
        {
          var row = y + j;
          for (int i = 0; i < blockWidth; i++)
          {
            var col = x + i;
            Image[col, row] = colour.ToPixel<Rgba32>();
          }
        }
      }
  }
}