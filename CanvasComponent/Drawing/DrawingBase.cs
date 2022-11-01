using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Drawing
{
    public abstract class DrawingBase : IDrawingStyle
    {
        internal protected bool ContinueDrawing { get; set; }

        internal event EventHandler<NewLinesEventArgs> NewLines;

        public virtual void OnMouseDown(Point lastPoint, Point current)
        {
            var lines = OnMouseMove(lastPoint, current);
            onNewLines(lines);
            ContinueDrawing = false;
        }

        public abstract List<Line> OnMouseMove(Point lastPoint, Point current);

        protected void onNewLines(IEnumerable<Line> lines)
            => NewLines?.Invoke(this, new NewLinesEventArgs(lines));
    }
}
