using CanvasComponent.DrawingStyles;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    public abstract class DrawByStyleBase : NotifyPropertyChanged, IDrawByStyle
    {
        internal protected virtual int SelectedDrawingStyle { get; set; }
        internal protected virtual bool SnapToGrid { get; set; }

        public virtual double GridDensity { get; set; } = 50;

        public virtual IEnumerable<Line> Lines { get; set; } = new List<Line>();

        protected virtual DrawingBase CurrentStyle { get; }

        protected virtual Point LastPoint { get; set; }

        internal protected virtual IDictionary<INamedValue<int>, DrawingBase> DrawingStyles { get; }

        public event EventHandler Draw;
        public event EventHandler<NewLinesEventArgs> NewLines;

        public abstract void MouseDown(MouseEventArgs e);
        public abstract void MouseMove(MouseEventArgs e);

        public abstract void Clear();

        protected virtual void OnDraw()
        {
            Draw?.Invoke(this, new());
        }

        protected virtual void OnNewLines(object sender, NewLinesEventArgs e)
        {
            NewLines?.Invoke(sender, e);
        }
    }
}