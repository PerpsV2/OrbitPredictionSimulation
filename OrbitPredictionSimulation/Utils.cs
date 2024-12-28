using System.Numerics;

namespace OrbitPredictionSimulation;

public static class Utils
{
    public static bool IsSqrt(BigInteger n, BigInteger root)
    {
        BigInteger lowerBound = root * root;
        BigInteger upperBound = (root + 1) * (root + 1);
        
        return n >= lowerBound && n <= upperBound;
    }
    
    public static decimal DecimalSqrt(decimal x, decimal epsilon = 0.0M)
    {
        if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

        decimal current = (decimal)Math.Sqrt((double)x), previous;
        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) / 2;
        }
        while (Math.Abs(previous - current) > epsilon);
        return current;
    }
}