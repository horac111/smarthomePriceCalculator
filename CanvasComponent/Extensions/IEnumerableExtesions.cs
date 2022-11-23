using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Extensions
{
    public static class IEnumerableExtesions
    {
        public static bool ContainsPoint(this IEnumerable<Line> lines, Point point)
        {
            int count = 0;
            foreach (Line line in lines)
            {
                if(line.IsPointOnLine(point))
                    return true;
                if(line.RayIntersect(point))
                    count++;
            }
            return count % 2 == 1;
        }

        public static Point SelectPointInside(this IEnumerable<Line> lines, IEnumerable<Point> points)
        =>  points.FirstOrDefault(x => lines.ContainsPoint(x));

        public static Line ClosestLine(this IEnumerable<Line> lines, Point point)
        {
            if(point == default)
                return null;
            Line closest = null;
            double closestDistance = double.PositiveInfinity;
            foreach (var line in lines)
            {
                var currentDistance = line.DistanceFromLine(point);
                if (currentDistance.NearlyLesser(closestDistance))
                {
                    closest = line;
                    closestDistance = currentDistance;
                }
            }
            return closest;
        }

        public static Point[] CloseLines(this IEnumerable<Line> lines, IList<Point> points, double maxDistance)
        {
            if (points is null || points.Count == 0)
                return new Point[0];
            Point[] result = points.ToArray();
            double[] distances = points.Select(x => double.PositiveInfinity).ToArray();
            foreach (var line in lines)
            {
                for(int i = 0; i < result.Length; i++)
                {
                    var currentDistance = line.DistanceFromLine(points[i]);
                    if (currentDistance.NearlyGreater(0) && currentDistance.NearlyLesser(maxDistance) &&
                        currentDistance.NearlyLesser(distances[i]))
                    {
                        result[i] = line.ClosestPointOnLine(points[i]);
                        var a = new Line(points[i], result[i]).Length();
                        distances[i] = currentDistance;
                    }
                }
                
            }
            return result;
        }

        public static IEnumerable<Line> ReplaceLasts(this IEnumerable<Line> baseLines, IEnumerable<Line> toReplce)
        {
            if (toReplce is null)
                return baseLines;
            return baseLines.Reverse().Skip(toReplce.Count()).Reverse().Concat(toReplce);
        }
    }
}
