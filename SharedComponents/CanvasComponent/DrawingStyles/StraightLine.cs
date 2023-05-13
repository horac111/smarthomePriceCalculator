using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.DrawingStyles
{
    internal class StraightLine : DrawingBase
    {

        public override List<Line> OnMouseMove(Point lastPoint, Point current)
        => new() { new(lastPoint, current) };
    }
}
