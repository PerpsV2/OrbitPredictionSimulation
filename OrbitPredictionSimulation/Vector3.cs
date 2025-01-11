namespace OrbitPredictionSimulation;

public struct Vector3(ScientificDecimal x, ScientificDecimal y, ScientificDecimal z)
{
    public static Vector3 Zero => new(0, 0, 0);
    public ScientificDecimal X { get; set; } = x;
    public ScientificDecimal Y { get; set; } = y;
    public ScientificDecimal Z { get; set; } = z;

    public static Vector3 operator -(Vector3 a) => new Vector3(-a.X, -a.Y, -a.Z);
    
    public static Vector3 operator +(Vector3 a, Vector3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(Vector3 a, Vector3 b)
        => a + -b;
    
    public static Vector3 operator *(Vector3 a, ScientificDecimal b) 
        => new (a.X * b, a.Y * b, a.Z * b);

    public static Vector3 operator /(Vector3 a, ScientificDecimal b)
    {
        if (b == 0) throw new DivideByZeroException();
        return new Vector3(a.X / b, a.Y / b, a.Z / b);
    }
    
    // dot product
    public static ScientificDecimal operator *(Vector3 a, Vector3 b)
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    public ScientificDecimal Magnitude()
        => ScientificDecimal.Sqrt(X * X + Y * Y + Z * Z);

    public static Vector3 CrossProduct(Vector3 a, Vector3 b)
        => new (a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X);

    public Vector2 Flatten()
    {
        return new Vector2(X, Y);
    }
    
    /*
    public double PrincipalAngle()
    {
        double angle = Math.Atan((double)(Y / X));
        if (X < 0 && Y > 0) return Math.PI + angle;
        if (X < 0 && Y < 0) return Math.PI + angle;
        if (X > 0 && Y < 0) return Math.Tau + angle;
        return angle;
    }*/

    /*
    public static double AngleBetween(Vector2 start, Vector2 end)
    {
        Vector2 difference = end - start;
        return difference.PrincipalAngle();
    }
    */
    
    public static double AngleBetween(Vector3 a, Vector3 b)
        => Math.Acos((double)(a * b / (a.Magnitude() * b.Magnitude())));

    public static Vector3 DirectionVectorBetween(Vector3 start, Vector3 end)
    {
        Vector3 difference = end - start;
        return difference / difference.Magnitude();
    }

    public override string ToString()
        => "<" + X + ", " + Y + ", " + Z + ">";
}