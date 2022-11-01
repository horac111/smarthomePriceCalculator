using CanvasComponent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Model
{
    
    public struct Point
    {
        public double X { get; init; }
        public double Y { get; init; }
        public Point(double x, double y) 
        {
           X = x;
           Y = y;
        }
        public static Point operator +(Point first, Point second)
            => new Point(first.X + second.X, first.Y + second.Y);

        public static Point operator -(Point first, Point second)
            => new Point(first.X - second.X, first.Y - second.Y);

        public override bool Equals(object obj)
        {
            return obj is Point point &&
                   X.NearlyEqual(point.X) &&
                   Y.NearlyEqual(point.Y);
        }

        public override int GetHashCode()
        {
            int a = (int)(X * (1 / DoubleExtensions.Epsilon));
            int b = (int)(Y * (1 / DoubleExtensions.Epsilon));
            return a + b;
        }

        public static bool  operator ==(Point first, Point second)
         => first.Equals(second);

        public static bool operator !=(Point first, Point second)
         => !first.Equals(second);

        public static Point operator *(Point first, double coeficient)
         => new(first.X * coeficient, first.Y * coeficient);
        public static Point operator /(Point first, double coeficient)
         => new(first.X / coeficient, first.Y / coeficient);

        public double Distance(Point second)
            => Math.Sqrt(Math.Pow(X - second.X, 2) + Math.Pow(Y - second.Y, 2));
    }
}
