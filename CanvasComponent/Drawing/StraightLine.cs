using CanvasComponent.Abstract;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Drawing
{
    internal class StraightLine : DrawingBase
    {

        public override List<Line> OnMouseMove(Point lastPoint, Point current)
        => new() { new(lastPoint, current)};
    }
}
