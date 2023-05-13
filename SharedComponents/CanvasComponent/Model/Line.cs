using CanvasComponent.Extensions;
using System;

namespace CanvasComponent.Model
{
    public class Line
    {
        public Line(Point start, Point end)
        {
            Start = start;
            End = end;
        }

        public Point Start { get; internal protected set; }

        public Point End { get; internal protected set; }

        public virtual double DistanceFromPoint(Point point)
        {
            var closest = ClosestPointOnLine(point);
            return closest.Distance(point);
        }

        public virtual Point PerpendicularPoint(Point point)
        {
            GetEquation(out double a, out double b, out double c);
            double temp = -2 * (a * point.X + b * point.Y + c) /
                               (a * a + b * b);
            double x = (temp * a + point.X);
            double y = (temp * b + point.Y);
            return (new Point(x, y) + point) / 2;
        }

        public virtual Point Intersection(Line line)
        {
            if (line.Start == Start || line.Start == End || line.End == Start || line.End == End)
                return default;
            GetEquation(out double a, out double b, out double c);
            line.GetEquation(out double d, out double e, out double f);

            double determinant = a * e - d * b;
            if (determinant.NearlyEqual(0))
                return default;
            double x = (b * f - e * c) / determinant;
            double y = (c * d - f * a) / determinant;
            Point result = new(x, y);
            if (IsPointOnLine(result) && line.IsPointOnLine(result))
                return result;

            return default;
        }

        public virtual Point ClosestPointOnLine(Point point)
        {
            var result = PerpendicularPoint(point);
            if (IsPointOnLine(result))
                return result;

            if (Start.Distance(result) < End.Distance(result))
                return Start;
            return End;
        }

        public virtual bool IsPointOnLine(Point point)
        {
            var dxc = point.X - Start.X;
            var dyc = point.Y - Start.Y;
            var dxl = End.X - Start.X;
            var dyl = End.Y - Start.Y;

            if (!(dxc * dyl - dyc * dxl).NearlyEqual(0))
                return false;

            if (Math.Abs(dxl).NearlyGreaterOrEqual(Math.Abs(dyl)))
                return dxl.NearlyGreater(0) ?
                  Start.X.NearlyLesserOrEqual(point.X) && point.X.NearlyLesserOrEqual(End.X) :
                  End.X.NearlyLesserOrEqual(point.X) && point.X.NearlyLesserOrEqual(Start.X);
            else
                return dyl.NearlyGreater(0) ?
                  Start.Y.NearlyLesserOrEqual(point.Y) && point.Y.NearlyLesserOrEqual(End.Y) :
                  End.Y.NearlyLesserOrEqual(point.Y) && point.Y.NearlyLesserOrEqual(Start.Y);
        }

        public virtual bool RayIntersect(Point point)
        {
            Point above, bellow;
            if (Start.Y.NearlyLesser(End.Y))
            {
                bellow = Start;
                above = End;
            }
            else
            {
                bellow = End;
                above = Start;
            }
            if (above.Y.NearlyEqual(point.Y) || bellow.Y.NearlyEqual(point.Y))
                point = new(point.X, point.Y + DoubleExtensions.Epsilon);

            if (point.Y.NearlyLesser(bellow.Y) || point.Y.NearlyGreater(above.Y) ||
                point.X.NearlyGreater(Math.Max(above.X, bellow.X)))
                return false;

            else if (point.X.NearlyLesser(Math.Min(above.X, bellow.X)))
                return true;
            else
            {
                double alpha, beta;
                if (!above.X.NearlyEqual(bellow.X))
                    alpha = (above.Y - bellow.Y) / (above.X - bellow.X);
                else
                    alpha = double.MaxValue;

                if (!above.X.NearlyEqual(point.X))
                    beta = (point.Y - bellow.Y) / (point.X - bellow.X);
                else
                    beta = double.MaxValue;
                return alpha.NearlyLesserOrEqual(beta);
            }
        }

        public virtual double Length()
            => Start.Distance(End);

        private void GetEquation(out double a, out double b, out double c)
        {
            a = End.Y - Start.Y;
            b = End.X - Start.X;
            c = a * Start.X - b * Start.Y;
            a = -a;
        }

        public virtual bool IsPointOnExtendedLine(Point point)
        {
            GetEquation(out double a, out double b, out double c);
            return (a * point.X + b * point.Y + c).NearlyEqual(0);
        }

        public override bool Equals(object obj)
        {
            return obj is Line line &&
                   (End == line.Start || Start == line.Start) &&
                   (End == line.End || Start == line.End);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public static bool operator ==(Line first, Line second)
         => first is not null ? first.Equals(second) : second is null;

        public static bool operator !=(Line first, Line second)
         => first is not null ? !first.Equals(second) : second is not null;
    }
}
