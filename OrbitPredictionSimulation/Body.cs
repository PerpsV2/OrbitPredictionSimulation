using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace OrbitPredictionSimulation;

public class Body(string name, BigDecimal mass, BigDecimal radius, Vector2 position, Vector2 velocity, SKColor color)
{
    private static readonly BigDecimal G = new BigDecimal(667430, -16);
    private static readonly int MinimumRadius = 10;
    private static readonly int CrossSpokeSize = 5;

    public string Name { get; set; } = name;
    public BigDecimal Mass { get; set; } = mass;
    public BigDecimal Radius { get; set; } = radius;
    public Vector2 Position { get; set; } = position;
    public Vector2 Velocity { get; set; } = velocity;
    public SKColor Color { get; set; } = color;
    public Body? Parent { get; }

    public Body(string name, BigDecimal mass, BigDecimal radius, Vector2 position, Vector2 velocity, SKColor color,
        Body? parent = null) : this(name, mass, radius, position, velocity, color)
    {
        if (parent == null) return;
        Parent = parent;
        Position = Parent.Position + position;
        Velocity = Parent.Velocity + velocity;
    }
    
    public void Draw(DrawOptions options)
    {
        SKPaint paint = new SKPaint { Color = Color };
        Camera cam = options.Camera;
        SKCanvas canvas = options.Canvas;
        float bodyScreenX = (float)((Position.X - cam.Left) / (cam.Right - cam.Left)) * options.ScreenSize.X;
        float bodyScreenY = (float)((Position.Y - cam.Top) / (cam.Bottom - cam.Top)) * options.ScreenSize.Y;
        float circleRadius = (float)(Radius / (cam.Right - cam.Left)) * options.ScreenSize.X;
        if (circleRadius < MinimumRadius)
        {
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX + CrossSpokeSize, bodyScreenY + CrossSpokeSize, paint);
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX + CrossSpokeSize, bodyScreenY - CrossSpokeSize, paint);
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX - CrossSpokeSize, bodyScreenY + CrossSpokeSize, paint);
            canvas.DrawLine(bodyScreenX, bodyScreenY, bodyScreenX - CrossSpokeSize, bodyScreenY - CrossSpokeSize, paint);
        }
        canvas.DrawCircle(bodyScreenX, bodyScreenY, circleRadius, paint);
    }

    public BigDecimal CalculateEccentricity()
    {
        if (Parent == null) throw new Exception("Cannot calculate eccentricity because no parent is set");
        BigDecimal mu = Parent.Mass * G;
        return (Vector2.CrossProduct(Velocity, Vector2.CrossProduct(Position, Velocity)) / mu - 
                Position / Position.Magnitude())
            .Magnitude();
    }

    public BigDecimal CalculateAngularMomentum()
    {
        return Vector2.CrossProduct(Position, Velocity * Mass);
    }
}