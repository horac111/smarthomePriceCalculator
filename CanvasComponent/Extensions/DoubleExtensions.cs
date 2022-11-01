using System;

namespace CanvasComponent.Extensions
{
    public static class DoubleExtensions
    {
        internal const double Epsilon = 0.0001;
        public static bool NearlyEqual(this double a, double b)
        {
            double diff = Math.Abs(a - b);

            if (a.Equals(b))
            { 
                return true;
            }
            return diff < Epsilon;
        }

        public static bool NearlyGreater(this double a, double b)
            => !a.NearlyEqual(b) && a > b;

        public static bool NearlyGreaterOrEqual(this double a, double b)
            => a.NearlyEqual(b) || a > b;

        public static bool NearlyLesser(this double a, double b)
            => !a.NearlyEqual(b) && a < b;

        public static bool NearlyLesserOrEqual(this double a, double b)
            => a.NearlyEqual(b) || a < b;
    }
}
