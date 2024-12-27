using System.Numerics;

namespace OrbitPredictionSimulation;

/// <summary>
/// Arbitrary precision decimal.
/// All operations are exact, except for division. Division never determines more digits than the given precision.
/// Source: https://gist.github.com/JcBernack/0b4eef59ca97ee931a2f45542b9ff06d
/// Based on https://stackoverflow.com/a/4524254
/// Author: Jan Christoph Bernack (contact: jc.bernack at gmail.com)
/// License: public domain
/// </summary>
public struct BigDecimal
    : IComparable
    , IComparable<BigDecimal>
{
    /// <summary>
    /// Specifies whether the significant digits should be truncated to the given precision after each operation.
    /// </summary>
    public static bool AlwaysTruncate = false;

    /// <summary>
    /// Sets the maximum precision of division operations.
    /// If AlwaysTruncate is set to true all operations are affected.
    /// </summary>
    public static int Precision = 20;

    public BigInteger Mantissa { get; set; }
    public int Exponent { get; set; }

    public BigDecimal(BigInteger mantissa, int exponent)
        : this()
    {
        Mantissa = mantissa;
        Exponent = exponent;
        Normalize();
        if (AlwaysTruncate)
        {
            Truncate();
        }
    }

    private BigDecimal(decimal mantissa, int exponent)
    {
        int decimals = (Decimal.GetBits(mantissa)[3] >> 16) & 0x7F;
        Mantissa = (BigInteger)(mantissa * (decimal)BigInteger.Pow(10, decimals));
        Exponent = exponent - decimals;
        Normalize();
        if (AlwaysTruncate)
        {
            Truncate();
        }
    }

    public static BigDecimal Create(decimal mantissa, int exponent)
    {
        return new BigDecimal(mantissa, exponent);
    }

    /// <summary>
    /// Removes trailing zeros on the mantissa
    /// </summary>
    public void Normalize()
    {
        if (Mantissa.IsZero)
        {
            Exponent = 0;
        }
        else
        {
            BigInteger remainder = 0;
            while (remainder == 0)
            {
                var shortened = BigInteger.DivRem(Mantissa, 10, out remainder);
                if (remainder == 0)
                {
                    Mantissa = shortened;
                    Exponent++;
                }
            }
        }
    }

    /// <summary>
    /// Truncate the number to the given precision by removing the least significant digits.
    /// </summary>
    /// <returns>The truncated number</returns>
    public BigDecimal Truncate(int precision)
    {
        // copy this instance (remember it's a struct)
        var shortened = this;
        // save some time because the number of digits is not needed to remove trailing zeros
        shortened.Normalize();
        // remove the least significant digits, as long as the number of digits is higher than the given Precision
        while (NumberOfDigits(shortened.Mantissa) > precision)
        {
            shortened.Mantissa /= 10;
            shortened.Exponent++;
        }
        // normalize again to make sure there are no trailing zeros left
        shortened.Normalize();
        return shortened;
    }

    public BigDecimal Truncate()
    {
        return Truncate(Precision);
    }

    public BigDecimal Floor()
    {
        return Truncate(NumberOfDigits(Mantissa) + Exponent);
    }

    public static int NumberOfDigits(BigInteger value)
    {
        // do not count the sign
        //return (value * value.Sign).ToString().Length;
        // faster version
        return (int)Math.Ceiling(BigInteger.Log10(value * value.Sign));
    }

    #region Conversions

    public static implicit operator BigDecimal(int value)
    {
        return new BigDecimal((BigInteger)value, 0);
    }

    public static implicit operator BigDecimal(double value)
    {
        var mantissa = (BigInteger) value;
        var exponent = 0;
        double scaleFactor = 1;
        while (Math.Abs(value * scaleFactor - (double)mantissa) > 0)
        {
            exponent -= 1;
            scaleFactor *= 10;
            mantissa = (BigInteger)(value * scaleFactor);
        }
        return new BigDecimal(mantissa, exponent);
    }

    public static implicit operator BigDecimal(decimal value)
    {
        var mantissa = (BigInteger)value;
        var exponent = 0;
        decimal scaleFactor = 1;
        while ((decimal)mantissa != value * scaleFactor)
        {
            exponent -= 1;
            scaleFactor *= 10;
            mantissa = (BigInteger)(value * scaleFactor);
        }
        return new BigDecimal(mantissa, exponent);
    }

    public static explicit operator double(BigDecimal value) => (double)value.Mantissa * Math.Pow(10, value.Exponent);

    public static explicit operator float(BigDecimal value) => Convert.ToSingle((double)value);

    public static explicit operator decimal(BigDecimal value) => 
        (decimal)value.Mantissa * (decimal)Math.Pow(10, value.Exponent);

    public static explicit operator int(BigDecimal value) =>
        (int)(value.Mantissa * BigInteger.Pow(10, value.Exponent));

    public static explicit operator uint(BigDecimal value) => 
        (uint)(value.Mantissa * BigInteger.Pow(10, value.Exponent));

    #endregion

    #region Operators

    public static BigDecimal operator +(BigDecimal value) => value;

    public static BigDecimal operator -(BigDecimal value)
    {
        value.Mantissa *= -1;
        return value;
    }

    public static BigDecimal operator ++(BigDecimal value) => value + 1;

    public static BigDecimal operator --(BigDecimal value) => value - 1;

    public static BigDecimal operator +(BigDecimal left, BigDecimal right) => Add(left, right);

    public static BigDecimal operator -(BigDecimal left, BigDecimal right) => Add(left, -right);

    private static BigDecimal Add(BigDecimal left, BigDecimal right)
    {
        return left.Exponent > right.Exponent
            ? new BigDecimal(AlignExponent(left, right) + right.Mantissa, right.Exponent)
            : new BigDecimal(AlignExponent(right, left) + left.Mantissa, left.Exponent);
    }

    public static BigDecimal operator *(BigDecimal left, BigDecimal right) => 
        new(left.Mantissa * right.Mantissa, left.Exponent + right.Exponent);

    public static BigDecimal operator /(BigDecimal dividend, BigDecimal divisor)
    {
        var exponentChange = Precision - (NumberOfDigits(dividend.Mantissa) - NumberOfDigits(divisor.Mantissa));
        if (exponentChange < 0)
        {
            exponentChange = 0;
        }
        dividend.Mantissa *= BigInteger.Pow(10, exponentChange);
        return new BigDecimal(dividend.Mantissa / divisor.Mantissa, dividend.Exponent - divisor.Exponent - exponentChange);
    }
		
    public static BigDecimal operator %(BigDecimal left, BigDecimal right)
    {
        return left - right * (left / right).Floor();
    }

    public static bool operator ==(BigDecimal left, BigDecimal right)
    {
        return left.Exponent == right.Exponent && left.Mantissa == right.Mantissa;
    }

    public static bool operator !=(BigDecimal left, BigDecimal right)
    {
        return left.Exponent != right.Exponent || left.Mantissa != right.Mantissa;
    }

    public static bool operator <(BigDecimal left, BigDecimal right)
    {
        return left.Exponent > right.Exponent ? AlignExponent(left, right) < right.Mantissa : left.Mantissa < AlignExponent(right, left);
    }

    public static bool operator >(BigDecimal left, BigDecimal right)
    {
        return left.Exponent > right.Exponent ? AlignExponent(left, right) > right.Mantissa : left.Mantissa > AlignExponent(right, left);
    }

    public static bool operator <=(BigDecimal left, BigDecimal right)
    {
        return left.Exponent > right.Exponent ? AlignExponent(left, right) <= right.Mantissa : left.Mantissa <= AlignExponent(right, left);
    }

    public static bool operator >=(BigDecimal left, BigDecimal right)
    {
        return left.Exponent > right.Exponent ? AlignExponent(left, right) >= right.Mantissa : left.Mantissa >= AlignExponent(right, left);
    }

    /// <summary>
    /// Returns the mantissa of value, aligned to the exponent of reference.
    /// Assumes the exponent of value is larger than of reference.
    /// </summary>
    private static BigInteger AlignExponent(BigDecimal value, BigDecimal reference)
    {
        return value.Mantissa * BigInteger.Pow(10, value.Exponent - reference.Exponent);
    }

    #endregion

    #region Additional mathematical functions

    public static BigDecimal Sqrt(BigDecimal value, int precision)
    {
        if (value.Mantissa == 0 && value.Exponent == 0) return 0;
        if (value.Mantissa > 0)
        {
            if (value.Exponent % 2 == 1)
            {
                value.Mantissa *= 10;
                value.Exponent--;
            }

            value.Exponent /= 2;
            BigInteger extendedMantissa = value.Mantissa * BigInteger.Pow(10, precision * 2);
            int bitLength = Convert.ToInt32(Math.Ceiling(BigInteger.Log(extendedMantissa, 2)));
            BigInteger root = BigInteger.One << (bitLength / 2);

            while (!Utils.isSqrt(extendedMantissa, root))
            {
                root += extendedMantissa / root;
                root /= 2;
            }

            value.Normalize();
            value.Exponent -= precision;
            return new BigDecimal(root, value.Exponent);
        }
        
        throw new ArithmeticException();
    }

    #endregion

    public override string ToString()
    {
        string mantissaString = Mantissa.ToString();
        if (Mantissa.ToString().Length > 30)
            mantissaString = (Mantissa / BigInteger.Parse("1" + String.Concat(Enumerable.Repeat("0", Mantissa.ToString().Length - 30)))).ToString();
        mantissaString = mantissaString.Insert(mantissaString[0] == '-' ? 2 : 1, ".");
        return string.Concat(mantissaString, "E", Exponent + Mantissa.ToString().Length - 1);
    }

    public bool Equals(BigDecimal other)
    {
        return other.Mantissa.Equals(Mantissa) && other.Exponent == Exponent;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
        {
            return false;
        }
        return obj is BigDecimal && Equals((BigDecimal) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (Mantissa.GetHashCode()*397) ^ Exponent;
        }
    }

    public int CompareTo(object? obj)
    {
        if (ReferenceEquals(obj, null) || !(obj is BigDecimal))
        {
            throw new ArgumentException();
        }
        return CompareTo((BigDecimal) obj);
    }

    public int CompareTo(BigDecimal other)
    {
        return this < other ? -1 : (this > other ? 1 : 0);
    }
}