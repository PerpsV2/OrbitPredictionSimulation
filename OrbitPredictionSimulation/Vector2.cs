namespace OrbitPredictionSimulation;

public class Vector2(ScientificDecimal x, ScientificDecimal y)
{
    public static Vector2 Zero => new(0, 0);
    public ScientificDecimal X { get; set; } = x;
    public ScientificDecimal Y { get; set; } = y;

    public static Vector2 operator -(Vector2 a) => new Vector2(-a.X, -a.Y);
    
    public static Vector2 operator +(Vector2 a, Vector2 b) 
        => new Vector2(a.X + b.X, a.Y + b.Y);

    public static Vector2 operator -(Vector2 a, Vector2 b)
        => a + -b;
    
    public static Vector2 operator *(Vector2 a, ScientificDecimal b) 
        => new Vector2(a.X * b, a.Y * b);

    public static Vector2 operator /(Vector2 a, ScientificDecimal b)
    {
        if (b == 0) throw new DivideByZeroException();
        return new Vector2(a.X / b, a.Y / b);
    }
    
    // dot product
    public static Vector2 operator *(Vector2 a, Vector2 b)
        => new Vector2(a.X * b.X, a.Y * b.Y);

    public ScientificDecimal Magnitude()
        => ScientificDecimal.Sqrt(X * X + Y * Y);
    
    public static ScientificDecimal CrossProduct(Vector2 a, Vector2 b)
        => a.X * b.Y - a.Y * b.X;
    
    public static Vector2 CrossProduct(Vector2 a, ScientificDecimal b)
        => new (a.Y * b, -a.X * b);

    public static Vector2 CrossProduct(ScientificDecimal a, Vector2 b)
        => new (-a * b.Y, a * b.X);

    public override string ToString()
        => "<" + X + ", " + Y + ">";
}