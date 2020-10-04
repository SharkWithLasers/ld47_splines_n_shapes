using System;

namespace KammBase
{
    public static class MathUtil
    {
        public static float GetNormalizedValue(
            float unnormValue, float unnormMin, float unnormMax, float normMin, float normMax)
        {
            return (normMax - normMin) * ((unnormValue - unnormMin) / (unnormMax - unnormMin)) + normMin;
        }

        public static float mod(float x, float m)
        {
            return (x % m + m) % m;
        }

        public static int mod(int x, int m)
        {
            return (x % m + m) % m;
        }
    }
} 

