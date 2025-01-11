using SkiaSharp;
using SKPoint = SkiaSharp.SKPoint;

namespace OrbitPredictionSimulation;

public class OrbitPath3D(List<Vector3?> points, SKColor color)
{
    private static readonly int PathWidth = Options.OrbitPathWidth;
    public List<Vector3?> Points { get; set; } = points;
    private SKPoint[] _screenPoints = [];
    public SKColor Color { get; set; } = color;
    public Body3D? Parent { get; set; }

    public void LogPosition(Body3D body, int maxPositions)
    {
        Points.Add(body.Position - (Parent?.Position ?? Vector3.Zero));
        if (Points.Count > maxPositions)
            Points.RemoveAt(0);
    }
    
    public void CalculateScreenPoints(DrawOptions options)
    {
        Camera cam = options.Camera;
        Vector3 origin = Parent?.RelativePosition ?? Vector3.Zero;
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
}