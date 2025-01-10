using SkiaSharp;

namespace OrbitPredictionSimulation;

public class Body(string name, ScientificDecimal mass, ScientificDecimal radius, Vector2 position, Vector2 velocity, 
    SKColor color)
{
    private static readonly ScientificDecimal G = new(667430, -16);
    private const int MinimumRadius = Options.MinimumVisibleRadius;
    private const int CrossSpokeSize = Options.CrossSpokeSize;
    private const int CrossSpokeWidth = Options.CrossSpokeWidth;
    private const int MaxPositions = Options.MaxOrbitPathPositions;

    public string Name { get; set; } = name;
    public ScientificDecimal Mass { get; set; } = mass;
    public ScientificDecimal Radius { get; set; } = radius;

    public Vector2 Position { get; set; } = position;
    public Vector2 RelativePosition => Position - (Parent?.Position ?? Vector2.Zero);
    
    public Vector2 Velocity { get; set; } = velocity;
    public Vector2 RelativeVelocity => Velocity - (Parent?.Velocity ?? Vector2.Zero);
    
    public SKColor Color { get; set; } = color;
    public Body? Parent { get; private set; }

    private double _lastLoggedAngle;
    private Vector2? _eccentricityVector;
    private ScientificDecimal _eccentricity;
    private double _periapsisTrueAnomaly;
    private double _apoapsisTrueAnomaly;
    private ScientificDecimal _specificAngularMomentum;
    private ScientificDecimal _specificOrbitalEnergy;
    private OrbitPath _orbitPath = new (new List<Vector2?>(), color);

    private ScientificDecimal Mu => G * (Parent?? throw new NullReferenceException()).Mass;

    private void CalculateInitials()
    {
        _specificAngularMomentum = Vector2.CrossProduct(RelativePosition, RelativeVelocity);
        _eccentricityVector = Vector2.CrossProduct(RelativeVelocity, _specificAngularMomentum) / Mu -
                              RelativePosition / RelativePosition.Magnitude();
        _eccentricity = _eccentricityVector.Value.Magnitude();
        _periapsisTrueAnomaly = _eccentricityVector.Value.PrincipalAngle();
        _apoapsisTrueAnomaly = _periapsisTrueAnomaly + Math.PI;
        _specificOrbitalEnergy = RelativeVelocity.Magnitude() * RelativeVelocity.Magnitude() * 0.5f - 
                                 Mu / RelativePosition.Magnitude();
    }
    
    public Body(string name, ScientificDecimal mass, ScientificDecimal radius, Vector2 position, Vector2 velocity, 
        SKColor color, Body? parent = null) 
        : this(name, mass, radius, position, velocity, color)
    {
        if (parent == null) return;
        Parent = parent;
        Position += parent.Position;
        Velocity += parent.Velocity;
        _orbitPath.Parent = parent;
        CalculateInitials();
        Console.WriteLine(Name + " " + _specificOrbitalEnergy);
    }
    
    public void Draw(DrawOptions options)
    {
        SKPaint paint = new SKPaint
        {
            Color = Color,
            StrokeWidth = CrossSpokeWidth,
            IsAntialias = true
        };
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

    public void SetRelativePosition(Vector2 position) 
        => Position = position + (Parent?.Position ?? Vector2.Zero);

    public void SetRelativeVelocity(Vector2 velocity)
        => Velocity = velocity + (Parent?.Velocity ?? Vector2.Zero);

    public void SetParent(Body? parent)
    {
        Parent = parent;
        _orbitPath.Parent = parent;
        _orbitPath.Points.Clear();
        LogNullPosition();
        if (parent != null)
            CalculateInitials();
    }
    
    #region Orbital Properties
    
    private ScientificDecimal PolarDistanceAtAnomaly(double angle)
    {
        if (_eccentricityVector == null) throw new NullReferenceException();
        ScientificDecimal constant = _specificAngularMomentum * _specificAngularMomentum / Mu;
        return constant / (1 + _eccentricityVector.Value.Magnitude() * Math.Cos(angle - _periapsisTrueAnomaly));
    }

    public Vector2 CartesianDistanceAtAnomaly(double angle)
    {
        angle += _apoapsisTrueAnomaly;
        ScientificDecimal distance = PolarDistanceAtAnomaly(angle);
        return new Vector2(distance * Math.Cos(angle), distance * Math.Sin(angle));
    }

    public ScientificDecimal OrbitalPeriod()
    {
        if (Parent == null) throw new NullReferenceException();
        ScientificDecimal constant = 4 * Math.PI * Math.PI / Mu;
        ScientificDecimal semiMajorAxis = PolarDistanceAtAnomaly(_periapsisTrueAnomaly); 
        return ScientificDecimal.Sqrt(constant * semiMajorAxis * semiMajorAxis * semiMajorAxis);
    }

    private double MeanAnomaly(ScientificDecimal time)
    {
        ScientificDecimal meanMotion = Math.Tau / OrbitalPeriod();
        return (double)(time * meanMotion);
    }

    private double EccentricAnomaly(ScientificDecimal time)
    {
        ScientificDecimal epsilon = new ScientificDecimal(1m, -2);
        ScientificDecimal meanAnomaly = MeanAnomaly(time);
        double solution = 0;
        while (ScientificDecimal.Abs(meanAnomaly - (solution - _eccentricity * Math.Sin(solution))) > epsilon)
        {
            solution = (double)(solution - (solution - _eccentricity * Math.Sin(solution) - meanAnomaly) / 
                (1 - _eccentricity * Math.Cos(solution)));
        }
        return solution;
    }

    public double TrueAnomaly(ScientificDecimal time)
    {
        double eccentricAnomaly = EccentricAnomaly(time);
        ScientificDecimal y = Math.Sin(eccentricAnomaly) * ScientificDecimal.Sqrt(1 - _eccentricity * _eccentricity);
        ScientificDecimal x = Math.Cos(eccentricAnomaly) - _eccentricity;
        return Math.Atan2((double) y, (double) x) % Math.Tau;
    }

    public Vector2 GetInstantGravitationalForce(Body attractor)
    {
        Vector2 difference = attractor.Position - Position;
        ScientificDecimal forceMagnitude = G * Mass * attractor.Mass / (difference.Magnitude() * difference.Magnitude());
        return Vector2.DirectionVectorBetween(Position, attractor.Position) * forceMagnitude;
    }
    
    // if this instance appears inside the list of attractors, skip over it
    public Vector2 GetInstantNetGravitationalForce(Body[] attractors)
    {
        Vector2 result = new Vector2(0, 0);
        return attractors
            .Where(attractor => attractor != this)
            .Aggregate(result, (current, attractor) => current + GetInstantGravitationalForce(attractor));
    }

    public Vector2 GetInstantAcceleration(Body[] attractors)
        => GetInstantNetGravitationalForce(attractors) / Mass;
    
    public bool IsInOrbit()
    {
        if (Parent == null) return false;
        ScientificDecimal eccentricity = (
            Vector2.CrossProduct(RelativeVelocity, Vector2.CrossProduct(RelativePosition, RelativeVelocity)) / 
            (Parent.Mass * G) - RelativePosition / RelativePosition.Magnitude()
        ).Magnitude();
        if (eccentricity > 1) return false;
        return true;
    }

    public ScientificDecimal GetSpecificOrbitalEnergy()
    {
        if (Parent == null) throw new NullReferenceException();
        ScientificDecimal speed = RelativeVelocity.Magnitude();
        ScientificDecimal distance = RelativePosition.Magnitude();
        return speed * speed * 0.5f - (Parent.Mass * G) / distance;
    }

    public void ResetSpecificOrbitalEnergy()
    {
        if (Parent == null) throw new NullReferenceException();
        ScientificDecimal distance = RelativePosition.Magnitude();
        if(2 * (_specificOrbitalEnergy + Parent.Mass * G / distance) < 0) return;
        ScientificDecimal desiredSpeed = ScientificDecimal.Sqrt(2 * (_specificOrbitalEnergy + Parent.Mass * G / distance));
        SetRelativeVelocity(RelativeVelocity / RelativeVelocity.Magnitude() * desiredSpeed);
    }
    
    #endregion
    
    #region Orbit Path Methods
    
    public void LogPosition()
        => _orbitPath.LogPosition(this, MaxPositions);

    public void LogNullPosition()
    {
        if (_orbitPath.Points.Count < 1) _orbitPath.Points.Add(null);
        else if (_orbitPath.Points[^1] != null) _orbitPath.Points.Add(null);
    }

    public void DrawOrbitPath(DrawOptions options)
        => _orbitPath.Draw(options);

    public void CalculateOrbitScreenPoints(DrawOptions options)
        => _orbitPath.CalculateScreenPoints(options);

    public double GetTrajectoryAngle(Vector2 futureLocation)
    {
        return Vector2.AngleBetween(RelativePosition, futureLocation);
    }

    public void LogTrajectory(Vector2 futureLocation)
    {
        _lastLoggedAngle = GetTrajectoryAngle(futureLocation);
    }

    public double GetAngularDeviationSinceLastLoggedPosition(Vector2 futureLocation)
    {
        return Math.Abs((GetTrajectoryAngle(futureLocation) - _lastLoggedAngle) % Math.Tau);
    }

    #endregion
}