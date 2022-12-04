using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    internal interface IDrawingStyle
    {
        List<Line> OnMouseMove(Point lastPoint, Point current);
        void OnMouseDown(Point lastPoint, Point current);
    }
}
