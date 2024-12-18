namespace OrbitPredictionSimulation;

public class Vector2(BigDecimal x, BigDecimal y)
{
    public static Vector2 Zero => new(0, 0);
    public BigDecimal X { get; set; } = x;
    public BigDecimal Y { get; set; } = y;

    public static Vector2 operator -(Vector2 a) => new Vector2(-a.X, -a.Y);
    
    public static Vector2 operator +(Vector2 a, Vector2 b) 
        => new Vector2(a.X + b.X, a.Y + b.Y);

    public static Vector2 operator -(Vector2 a, Vector2 b)
        => a + -b;
    
    public static Vector2 operator *(Vector2 a, BigDecimal b) 
        => new Vector2(a.X * b, a.Y * b);

    public static Vector2 operator /(Vector2 a, BigDecimal b)
    {
        if (b == 0) throw new DivideByZeroException();
        return new Vector2(a.X / b, a.Y / b);
    }
    
    // dot product
    public static Vector2 operator *(Vector2 a, Vector2 b)
        => new Vector2(a.X * b.X, a.Y * b.Y);
}