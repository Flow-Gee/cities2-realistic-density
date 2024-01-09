using RealisticDensity.Helper;
using System;
using Unity.Mathematics;

namespace RealisticDensity.Common
{
    public static class Calc
    {
        public static float DecimalFloat(float value)
        {
            return (float)Math.Round(value, 2);
        }
    }
}
