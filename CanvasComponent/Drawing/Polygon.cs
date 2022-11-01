using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Drawing
{
    class Polygon : StraightLine
    {
        public override void OnMouseDown(Point lastPoint, Point current)
            => onNewLines(OnMouseMove(lastPoint, current));
        
    }
}
