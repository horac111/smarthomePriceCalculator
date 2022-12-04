using CanvasComponent.Model;
using System.Collections.Generic;

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
