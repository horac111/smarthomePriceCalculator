using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    internal interface IDrawingStyle
    {
        List<Line> OnMouseMove(Point lastPoint, Point current);
        void OnMouseDown(Point lastPoint, Point current);
    }
}
