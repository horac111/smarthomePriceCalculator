using CanvasComponent.Model;

namespace CanvasComponent.DrawingStyles
{
    class Polygon : StraightLine
    {
        public override void OnMouseDown(Point lastPoint, Point current)
            => OnNewLines(OnMouseMove(lastPoint, current));

    }
}
