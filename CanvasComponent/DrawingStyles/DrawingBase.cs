using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;

namespace CanvasComponent.DrawingStyles
{
    public abstract class DrawingBase : IDrawingStyle
    {
        internal protected bool ContinueDrawing { get; set; }

        internal event EventHandler<NewLinesEventArgs> NewLines;

        public virtual void OnMouseDown(Point lastPoint, Point current)
        {
            var lines = OnMouseMove(lastPoint, current);
            OnNewLines(lines);
            ContinueDrawing = false;
        }

        public abstract List<Line> OnMouseMove(Point lastPoint, Point current);

        protected void OnNewLines(IEnumerable<Line> lines)
            => NewLines?.Invoke(this, new NewLinesEventArgs(lines));
    }
}
