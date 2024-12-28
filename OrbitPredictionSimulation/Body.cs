using System.Drawing;
using System.Numerics;
using Silk.NET.OpenGL;
using SkiaSharp;

namespace OrbitPredictionSimulation;

public class Body(string name, ScientificDecimal mass, ScientificDecimal radius, Vector2 position, Vector2 velocity, 
    float trueAnomaly, SKColor color)
{
    private static readonly ScientificDecimal G = new(667430, -16);
    private const int MinimumRadius = 10;
    private const int CrossSpokeSize = 5;
    private const int MaxPositions = 200;

    public string Name { get; set; } = name;
    public ScientificDecimal Mass { get; set; } = mass;
    public ScientificDecimal Radius { get; set; } = radius;
    public Vector2 Position { get; set; } = position;
    public Vector2 AbsolutePosition => Position + (Parent?.AbsolutePosition ?? Vector2.Zero);
    public Vector2 Velocity { get; set; } = velocity;
    public float TrueAnomaly { get; set; } = trueAnomaly;
    public SKColor Color { get; set; } = color;
    public Body? Parent { get; }
    private readonly Vector2? _eccentricityVector;
    private readonly ScientificDecimal _specificAngularMomentum;
    private readonly OrbitPath _orbitPath = new OrbitPath(new List<Vector2>(), color);

    private ScientificDecimal Mu => G * (Parent?? throw new NullReferenceException()).Mass;

    public Body(string name, ScientificDecimal mass, ScientificDecimal radius, Vector2 position, Vector2 velocity, 
        float trueAnomaly, SKColor color, Body? parent = null) 
        : this(name, mass, radius, position, velocity, trueAnomaly, color)
    {
        if (parent == null) return;
        Parent = parent;
        _orbitPath.Parent = parent;
        _eccentricityVector = Vector2.CrossProduct(Velocity, Vector2.CrossProduct(Position, Velocity)) / Mu -
                              Position / Position.Magnitude();
        _specificAngularMomentum = Vector2.CrossProduct(Position, Velocity);
    }
    
    public void Draw(DrawOptions options)
    {
        SKPaint paint = new SKPaint { Color = Color };
        Camera cam = options.Camera;
        SKCanvas canvas = options.Canvas;
        float bodyScreenX = (float)((AbsolutePosition.X - cam.Left) / (cam.Right - cam.Left)) * options.ScreenSize.X;
        float bodyScreenY = (float)((AbsolutePosition.Y - cam.Top) / (cam.Bottom - cam.Top)) * options.ScreenSize.Y;
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
    

    public ScientificDecimal PolarDistanceAtAnomaly(float angle)
    {
        if (_eccentricityVector == null) throw new NullReferenceException();
        ScientificDecimal constant = _specificAngularMomentum * _specificAngularMomentum / Mu;
        double periapsisTrueAnomaly = Math.Atan((double)(_eccentricityVector.Y / _eccentricityVector.X));
        if (_eccentricityVector.X < 0 && _eccentricityVector.Y > 0) periapsisTrueAnomaly = Math.PI - periapsisTrueAnomaly;
        if (_eccentricityVector.X < 0 && _eccentricityVector.Y < 0) periapsisTrueAnomaly += Math.PI;
        if (_eccentricityVector.X > 0 && _eccentricityVector.Y < 0) periapsisTrueAnomaly += Math.PI;
        return constant / (1 - _eccentricityVector.Magnitude() * Math.Cos(angle - periapsisTrueAnomaly));
    }

    public Vector2 CartesianDistanceAtAnomaly(float angle)
    {
        ScientificDecimal distance = PolarDistanceAtAnomaly(angle);
        return new Vector2(distance * Math.Cos(angle), distance * Math.Sin(angle));
    }

    public void LogPosition()
    {
        _orbitPath.Points.Add(Position);
        if (_orbitPath.Points.Count > MaxPositions)
            _orbitPath.Points.RemoveAt(0);
    }

    public void DrawOrbitPath(DrawOptions options)
    {
        _orbitPath.Draw(options);
    }

    public void CalculateOrbitScreenPoints(DrawOptions options)
    {
        _orbitPath.CalculateScreenPoints(options);
    }
}