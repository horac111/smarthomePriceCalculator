using CanvasComponent.Abstract;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Drawing
{
    internal class Rectangle : DrawingBase
    {
        public override List<Line> OnMouseMove(Point lastPoint, Point current)
        {
            List<Line> lines = new();
            lines.Add(new(lastPoint, current with { Y = lastPoint.Y }));
            lines.Add(new(lastPoint, current with { X = lastPoint.X }));
            lines.Add(new(lastPoint with { Y = current.Y }, current));
            lines.Add(new(lastPoint with { X = current.X }, current));
            return lines;
        }
    }
}
