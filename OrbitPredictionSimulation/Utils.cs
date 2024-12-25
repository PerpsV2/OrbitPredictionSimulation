using System.Numerics;

namespace OrbitPredictionSimulation;

public static class Utils
{
    public static bool isSqrt(BigInteger n, BigInteger root)
    {
        BigInteger lowerBound = root * root;
        BigInteger upperBound = (root + 1) * (root + 1);
        
        return n >= lowerBound && n <= upperBound;
    }
}