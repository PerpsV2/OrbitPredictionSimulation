using SkiaSharp;
using SKPoint = SkiaSharp.SKPoint;

namespace OrbitPredictionSimulation;

public class OrbitPath(List<Vector2> points, SKColor color)
{
    private static readonly int PathWidth = 2;
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
        List<SKPoint> screenPoints = new List<SKPoint>();
        Vector2 origin = Parent?.Position ?? Vector2.Zero;
        SKPoint? previousPoint = null;
        for (int i = 0; i < Points.Count; i++)
        {
            Vector2? worldPoint = Points[i];
            SKPoint? potentialScreenPoint = worldPoint != null ? new SKPoint(
                (float)((worldPoint.X - cam.Left + origin.X) / cam.Width) * options.ScreenSize.X, 
                (float)((worldPoint.Y - cam.Top + origin.Y) / cam.Height) * options.ScreenSize.Y
            ) : null;
            if (potentialScreenPoint == null)
            {
                previousPoint = null;
                continue;
            }
            SKPoint screenPoint = potentialScreenPoint.Value;
            if (screenPoint.X > 0 && screenPoint.X <= options.ScreenSize.X &&
                screenPoint.Y > 0 && screenPoint.Y <= options.ScreenSize.Y)
            {
                screenPoints.Add(screenPoint);
                screenPoints.Add(previousPoint ?? screenPoint);
            }
            previousPoint = new SKPoint(screenPoint.X, screenPoint.Y);
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
        return Vector2.AngleBetween(Points[^2]!, Points[^1]!);
    }
    
    public double? GetTrajectoryPathAngularDeviation(Vector2 futureLocation)
    {
        if (Points.Count < 2) return null;
        if (Points[^1] == null) return null;
        double angle = Vector2.AngleBetween(Points[^1]!, futureLocation);
        double? latestPathHeading = GetLatestPathHeading();
        return latestPathHeading == null ? null : Math.Abs((double)(angle - latestPathHeading));
    }
}