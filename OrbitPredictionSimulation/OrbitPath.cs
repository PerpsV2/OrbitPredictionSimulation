using SkiaSharp;
using SKPoint = SkiaSharp.SKPoint;

namespace OrbitPredictionSimulation;

public class OrbitPath(List<Vector2> points, SKColor color)
{
    public List<Vector2> Points { get; set; } = points;
    private SKPoint[] _screenPoints = [];
    public SKColor Color { get; set; } = color;
    public Body? Parent { get; set; }
    
    public void CalculateScreenPoints(DrawOptions options)
    {
        Camera cam = options.Camera;
        List<SKPoint> screenPoints = new List<SKPoint>();
        Vector2 origin = Parent?.Position ?? Vector2.Zero;
        foreach (Vector2 worldPoint in Points)
        {
            Console.WriteLine(worldPoint.X.Exponent);
            Console.WriteLine(cam.Width.Exponent);
            Console.WriteLine(Math.Abs(worldPoint.X.Exponent - cam.Width.Exponent));
            if (Math.Abs(worldPoint.X.Exponent - cam.Width.Exponent) > 2) continue;
            SKPoint screenPoint = new SKPoint(
                (float)((worldPoint.X - cam.Left + origin.X) / cam.Width) * options.ScreenSize.X, 
                (float)((worldPoint.Y - cam.Top + origin.Y) / cam.Height) * options.ScreenSize.Y
            );
            if (screenPoint.X > 0 && screenPoint.X <= options.ScreenSize.X &&
                screenPoint.Y > 0 && screenPoint.Y <= options.ScreenSize.Y)
            {
                screenPoints.Add(screenPoint);
                if (screenPoints.Count > 1) screenPoints.Add(screenPoint);
            }
            GC.Collect();
        }
        _screenPoints = screenPoints.ToArray();
    }
    
    public void Draw(DrawOptions options)
    {
        options.Canvas.DrawPoints(SKPointMode.Lines, _screenPoints, options.Paint);
    }
}