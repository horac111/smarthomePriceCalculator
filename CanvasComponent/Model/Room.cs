using CanvasComponent.Abstract;
using CanvasComponent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Model
{
    public class Room
    {
        public Room(IEnumerable<Line> lines)
        {
            Lines = lines.ToList().AsReadOnly();
            Center = calculateCenter();
            Size = calculateSize();
        }

        public IReadOnlyList<Line> Lines { get; }

        public Point Center { get; }

        public ICollection<ISmartDevice> Devices { get; } = new List<ISmartDevice>();

        public double Size { get;  }

        public string Name { get; set; }

        public ICollection<Room> Insiders  { get; } = new HashSet<Room>();

        public override bool Equals(object obj)
        {
            if(obj is Room room)
            {
                var myPoints = getAllPoints();
                var roomPoints = room.getAllPoints();
                return Center.X.NearlyEqual(room.Center.X)
                   && Center.Y.NearlyEqual(room.Center.Y)
                   && myPoints.Count() == roomPoints.Count() 
                   && myPoints.Intersect(roomPoints).Count() == myPoints.Count();
            }
            return false;
            
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Center, getAllPoints().Sum(x => x.GetHashCode()));
        }

        protected virtual Point calculateCenter()
        {
            var allPoints = getAllPoints();
            double x = allPoints.Sum(x => x.X) / allPoints.Count();
            double y = allPoints.Sum(x => x.Y) / allPoints.Count();
            Point candidate = new(x, y);
            if(Lines.ContainsPoint(candidate))
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

        protected virtual double calculateSize()
            => Math.Abs(Lines.Sum(x => (x.Start.X + x.End.X) * (x.Start.Y - x.End.Y))) / 2;

        public virtual bool Contains(Point point)
            => Insiders.All(x => !x.Contains(point)) && Lines.ContainsPoint(point);

        public virtual bool Contains(Room room)
        {
            if(!Lines.ContainsPoint(room.Center))
                return false;
            return room.getAllPoints().All(x => Lines.ContainsPoint(x));
        }

        protected virtual IEnumerable<Point> getAllPoints()
         => Lines.SelectMany(x => new[] { x.Start, x.End }).Distinct();
    }
}
