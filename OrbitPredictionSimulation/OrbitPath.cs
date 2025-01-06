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
        SKPoint? previousPoint = null;
        for (int i = 0; i < Points.Count; i++)
        {
            Vector2 worldPoint = Points[i];
            SKPoint screenPoint = new SKPoint(
                (float)((worldPoint.X - cam.Left + origin.X) / cam.Width) * options.ScreenSize.X, 
                (float)((worldPoint.Y - cam.Top + origin.Y) / cam.Height) * options.ScreenSize.Y
            );
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
        SKPaint paint = new SKPaint { Color = new SKColor(Color.Red, Color.Green, Color.Blue, 125) };
        options.Canvas.DrawPoints(SKPointMode.Lines, _screenPoints, paint);
    }
}