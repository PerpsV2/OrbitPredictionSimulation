using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace OrbitPredictionSimulation;

public class Body(BigDecimal mass, BigDecimal radius, Vector2 position, Vector2 velocity)
{
    private static readonly int MinimumRadius = 10;
    private static readonly int CrossSpokeSize = 5;
    
    public BigDecimal Mass { get; set; } = mass;
    public BigDecimal Radius { get; set; } = radius;
    public Vector2 Position { get; set; } = position;
    public Vector2 Velocity { get; set; } = velocity;

    public void Draw(DrawOptions options)
    {
        Camera cam = options.Camera;
        SKCanvas canvas = options.Canvas;
        float bodyScreenX = (float)((Position.X - cam.Left) / (cam.Right - cam.Left)) * options.ScreenSize.X;
        float bodyScreenY = (float)((Position.Y - cam.Top) / (cam.Bottom - cam.Top)) * options.ScreenSize.Y;
        float circleRadius = (float)(Radius / (cam.Right - cam.Left)) * options.ScreenSize.X;
        if (circleRadius < MinimumRadius)
        {
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX + CrossSpokeSize, bodyScreenY + CrossSpokeSize, options.Paint);
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX + CrossSpokeSize, bodyScreenY - CrossSpokeSize, options.Paint);
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX - CrossSpokeSize, bodyScreenY + CrossSpokeSize, options.Paint);
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX - CrossSpokeSize, bodyScreenY - CrossSpokeSize, options.Paint);
        }
        canvas.DrawCircle(bodyScreenX, bodyScreenY, circleRadius, options.Paint);
    }
}