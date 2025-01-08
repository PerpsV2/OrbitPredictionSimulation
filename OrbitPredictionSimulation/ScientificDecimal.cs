namespace OrbitPredictionSimulation;

/// <summary>
/// Number with decimal precision but arbitrary place value
/// </summary>
public struct ScientificDecimal
    : IComparable, IComparable<ScientificDecimal>, IEquatable<ScientificDecimal>
{
    public const int PrintPrecision = 5;
    
    public decimal Mantissa { get; set; }
    public int Exponent { get; set; }
    public bool Positive => decimal.IsPositive(Mantissa);

    public ScientificDecimal(decimal mantissa, int exponent)
        : this()
    {
        Mantissa = mantissa;
        Exponent = exponent;
        Normalize();
    }

    /// <summary>
    /// Sets the largest non-zero digit of the mantissa to be in the ones place
    /// </summary>
    private ScientificDecimal Normalize()
    {
        if (Mantissa == 0)
        {
            Exponent = 0;
            return this;
        }
        
        while (Math.Abs(Mantissa) >= 10)
        {
            Mantissa /= 10;
            Exponent++;
        }

        while (Math.Abs(Mantissa) < 1)
        {
            Mantissa *= 10;
            Exponent--;
        }

        return this;
    }

    public ScientificDecimal IncreaseExponent(int exponent)
    {
        int exponentDifference = exponent - Exponent;
        if (exponentDifference < 0) throw new ArgumentOutOfRangeException();
        if (exponentDifference == 0) return Normalize();
        while (Exponent != exponent)
        {
            Mantissa /= 10;
            Exponent++;
        }
        return this;
    }
    
    #region Conversions

    public static implicit operator ScientificDecimal(int value) 
        => new ScientificDecimal(value, 0);

    public static implicit operator ScientificDecimal(double value)
        => new ScientificDecimal((decimal)value, 0);
    
    public static implicit operator ScientificDecimal(decimal value) 
        => new ScientificDecimal(value, 0);

    public static explicit operator double(ScientificDecimal value)
        => (double)value.Mantissa * Math.Pow(10, value.Exponent);
    
    public static explicit operator float(ScientificDecimal value)
        => Convert.ToSingle((double)value);
    
    public static explicit operator int (ScientificDecimal value)
        => (int)value.Mantissa * (int)Math.Pow(10, value.Exponent);
    
    public static explicit operator uint (ScientificDecimal value)
        => (uint)value.Mantissa * (uint)Math.Pow(10, value.Exponent);
    
    #endregion
    
    #region Operators

    private static ScientificDecimal Add(ScientificDecimal left, ScientificDecimal right)
    {
        return (left.Exponent > right.Exponent ? 
            new ScientificDecimal(right.IncreaseExponent(left.Exponent).Mantissa + left.Mantissa, left.Exponent) :
            new ScientificDecimal(left.IncreaseExponent(right.Exponent).Mantissa + right.Mantissa, right.Exponent))
            .Normalize();
    }

    private static ScientificDecimal Multiply(ScientificDecimal left, ScientificDecimal right)
         => new ScientificDecimal(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent).Normalize();

    private static ScientificDecimal Divide(ScientificDecimal dividend, ScientificDecimal divisor)
        => new ScientificDecimal(dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent).Normalize();
    
    public static ScientificDecimal operator +(ScientificDecimal value) => value;
    
    public static ScientificDecimal operator -(ScientificDecimal value) 
        => new(-value.Mantissa, value.Exponent);

    public static ScientificDecimal operator +(ScientificDecimal left, ScientificDecimal right)
        => Add(left, right);

    public static ScientificDecimal operator -(ScientificDecimal left, ScientificDecimal right)
        => Add(left, -right);

    public static ScientificDecimal operator ++(ScientificDecimal value)
        => Add(value, 1);
    
    public static ScientificDecimal operator --(ScientificDecimal value)
        => Add(value, -1);
    
    public static ScientificDecimal operator*(ScientificDecimal left, ScientificDecimal right)
        => Multiply(left, right);
    
    public static ScientificDecimal operator/(ScientificDecimal dividend, ScientificDecimal divisor)
        => Divide(dividend, divisor);
    
    public static bool operator ==(ScientificDecimal left, ScientificDecimal right)
        => left.Mantissa == right.Mantissa && left.Exponent == right.Exponent;

    public static bool operator !=(ScientificDecimal left, ScientificDecimal right) 
        => !(left == right);

    public static bool operator <(ScientificDecimal left, ScientificDecimal right)
    {
        if (left.Positive && !right.Positive) return false;
        if (!left.Positive && right.Positive) return true;
        bool result = left.Exponent == right.Exponent ? left.Mantissa < right.Mantissa : left.Exponent < right.Exponent;
        if (left.Positive && right.Positive) return result;
        return !result;
    }
    
    public static bool operator >(ScientificDecimal left, ScientificDecimal right)
    {
        if (left.Positive && !right.Positive) return true;
        if (!left.Positive && right.Positive) return false;
        bool result = left.Exponent == right.Exponent ? left.Mantissa > right.Mantissa : left.Exponent > right.Exponent;
        if (left.Positive && right.Positive) return result;
        return !result;
    }
    
    public static bool operator <=(ScientificDecimal left, ScientificDecimal right)
        => left < right || left == right;
    
    public static bool operator >=(ScientificDecimal left, ScientificDecimal right)
        => left > right || left == right;

    public static ScientificDecimal Sqrt(ScientificDecimal value)
    {
        if (value.Mantissa < 0) throw new ArgumentOutOfRangeException();
        if (value.Exponent % 2 != 0) value = value.IncreaseExponent(value.Exponent + 1);
        return new ScientificDecimal(Utils.DecimalSqrt(value.Mantissa), value.Exponent / 2);
    }

    public static ScientificDecimal Abs(ScientificDecimal value)
        => new (Math.Abs(value.Mantissa), value.Exponent);   
    
    #endregion
    
    public override string ToString()
    {
        return Mantissa + "e" + Exponent.ToString("+0;-#");;
    }

    public int CompareTo(object? obj)
    {
        if (obj is not ScientificDecimal @decimal) 
            throw new ArgumentException($"Object must be of type {nameof(ScientificDecimal)}");
        return CompareTo(@decimal);
    }

    public int CompareTo(ScientificDecimal other)
        => this < other ? -1 : this > other ? 1 : 0;

    public bool Equals(ScientificDecimal other)
    {
        return Mantissa == other.Mantissa && Exponent == other.Exponent;
    }

    public override bool Equals(object? obj)
    {
        return obj is ScientificDecimal other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Mantissa, Exponent);
    }
}