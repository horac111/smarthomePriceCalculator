using CanvasComponent.Abstract;
using CanvasComponent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CanvasComponent.Model
{
    public class Room : INamed
    {
        public Room(IEnumerable<Line> lines)
        {
            Lines = lines.ToList().AsReadOnly();
            Size = CalculateSize();
            Center = CalculateCenter();
        }

        internal Room(IEnumerable<Line> lines, IEnumerable<ISmartDevice> devices, string name)
            :this(lines)
        {
            Devices = devices.ToList();
            Name = name;
        }

        public IReadOnlyList<Line> Lines { get; }

        public Point Center { get; }

        public ICollection<ISmartDevice> Devices { get; } = new List<ISmartDevice>();

        public double Size { get; }

        public string Name { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Room room)
            {
                if (!Center.X.NearlyEqual(room.Center.X)
                   || !Center.Y.NearlyEqual(room.Center.Y))
                    return false;
                var myPoints = GetAllPoints();
                var roomPoints = room.GetAllPoints();
                return myPoints.Count() == roomPoints.Count()
                   && myPoints.Intersect(roomPoints).Count() == myPoints.Count();
            }
            return false;

        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Center, GetAllPoints().Sum(x => x.GetHashCode()));
        }

        protected virtual Point CalculateCenter()
        {
            var allPoints = GetAllPoints();
            var centroid = GetCentroid(allPoints);
            if (centroid != default && Lines.ContainsPoint(centroid))
                return centroid;
            double x = allPoints.Sum(x => x.X) / allPoints.Count();
            double y = allPoints.Sum(x => x.Y) / allPoints.Count();
            Point candidate = new(x, y);
            if (Lines.ContainsPoint(candidate))
                return candidate;
            var closestLine = Lines.ClosestLine(candidate);
            var point = closestLine.PerpendicularPoint(candidate);
            Point[] points = new[]
            {
                point + new Point(20, 0),
                point + new Point(-20, 0),
                point + new Point(20, 20),
                point + new Point(-20, -20),
                point + new Point(0, 20),
                point + new Point(0, -20),
            };
            var inside = Lines.SelectPointInside(points);
            if (inside != default)
                return inside;
            return point;
        }

        private Point GetCentroid(IEnumerable<Point> allPoints)
        {
            if (Size == default || allPoints is null || !allPoints.Any())
                return default;


            double X = 0;
            double Y = 0;
            double secondFactor;
            Point previousPoint = allPoints.Last();
            foreach (Point point in allPoints)
            {
                secondFactor = previousPoint.X * point.Y - point.X * previousPoint.Y;
                X += (previousPoint.X + point.X) * secondFactor;
                Y += (previousPoint.Y + point.Y) * secondFactor;
                previousPoint = point;
            }

            X /= (6 * Size);
            Y /= (6 * Size);

            if (X < 0)
            {
                X = -X;
                Y = -Y;
            }

            return new Point(X, Y);
        }

        protected virtual double CalculateSize()
            => Math.Abs(Lines.Sum(x => (x.Start.X + x.End.X) * (x.Start.Y - x.End.Y))) / 2;

        public virtual bool Contains(Point point)
            => Lines.ContainsPoint(point);

        public virtual bool Contains(Room room)
        {
            if (!Lines.ContainsPoint(room.Center))
                return false;
            return room.GetAllPoints().All(x => Lines.ContainsPoint(x));
        }

        public virtual bool Equals(IEnumerable<Point> points)
        {
            if (points is null)
                return false;
            return GetAllPoints().SequenceEqual(points);
        }

        internal protected virtual IEnumerable<Point> GetAllPoints()
         => Lines.SelectMany(x => new[] { x.Start, x.End }).Distinct();

        public static bool operator ==(Room first, Room second)
         => first is not null ? first.Equals(second) : second is null;

        public static bool operator !=(Room first, Room second)
         => first is not null ? !first.Equals(second) : second is null;
    }
}
