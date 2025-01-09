using SkiaSharp;
using SKPoint = SkiaSharp.SKPoint;

namespace OrbitPredictionSimulation;

public class OrbitPath(List<Vector2?> points, SKColor color)
{
    private static readonly int PathWidth = Options.OrbitPathWidth;
    public List<Vector2?> Points { get; set; } = points;
    private SKPoint[] _screenPoints = [];
    public SKColor Color { get; set; } = color;
    public Body? Parent { get; set; }

    public void LogPosition(Body body, int maxPositions)
    {
        Points.Add(body.AbsolutePosition - (Parent?.AbsolutePosition ?? Vector2.Zero));
        if (Points.Count > maxPositions)
            Points.RemoveAt(0);
    }
    
    public void CalculateScreenPoints(DrawOptions options)
    {
        Camera cam = options.Camera;
        Vector2 origin = Parent?.Position ?? Vector2.Zero;
        SKPoint? previousPoint = null;
        List<SKPoint> screenPoints = new List<SKPoint>();
        foreach (var worldPoint in Points)
        {
            SKPoint? screenPoint = worldPoint != null ? new SKPoint(
                (float)((worldPoint.Value.X - cam.Left + origin.X) / cam.Width) * options.ScreenSize.X, 
                (float)((worldPoint.Value.Y - cam.Top + origin.Y) / cam.Height) * options.ScreenSize.Y
            ) : null;
            
            if (screenPoint == null)
            {
                previousPoint = null;
                continue;
            }
            
            if (screenPoint.Value.X > 0 && screenPoint.Value.X <= options.ScreenSize.X &&
                screenPoint.Value.Y > 0 && screenPoint.Value.Y <= options.ScreenSize.Y)
            {
                screenPoints.Add(screenPoint.Value);
                screenPoints.Add(previousPoint ?? screenPoint.Value);
            }
            
            previousPoint = new SKPoint(screenPoint.Value.X, screenPoint.Value.Y);
        }

        _screenPoints = screenPoints.ToArray();
    }
    
    public void Draw(DrawOptions options)
    {
        SKPaint paint = new SKPaint
        {
            Color = new SKColor(Color.Red, Color.Green, Color.Blue, 125),
            StrokeWidth = PathWidth,
            IsAntialias = true
        };
        options.Canvas.DrawPoints(SKPointMode.Lines, _screenPoints, paint);
    }

    public double? GetLatestPathHeading()
    {
        if (Points.Count < 2) return null;
        if (Points[^2] == null || Points[^1] == null) return null;
        return Vector2.AngleBetween(Points[^2]!.Value, Points[^1]!.Value);
    }
    
    public double? GetTrajectoryPathAngularDeviation(Vector2 futureLocation)
    {
        if (Points.Count < 2) return null;
        if (Points[^1] == null) return null;
        double angle = Vector2.AngleBetween(Points[^1]!.Value, futureLocation);
        double? latestPathHeading = GetLatestPathHeading();
        Console.WriteLine((angle, latestPathHeading));
        return latestPathHeading == null ? null : Math.Abs((double)(angle - latestPathHeading) % Math.Tau);
    }
}