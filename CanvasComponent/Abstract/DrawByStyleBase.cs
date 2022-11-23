using CanvasComponent.Drawing;
using CanvasComponent.Enums;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    public abstract class DrawByStyleBase : NotifyPropertyChanged
    {
        internal protected virtual int SelectedDrawingStyle { get; set; }
        internal protected virtual bool SnapToGrid { get; set; }

        internal protected virtual double GridSize { get; set; }

        internal protected virtual IEnumerable<Line> Lines { get; set; } = new List<Line>();

        internal protected virtual DrawingBase CurrentStyle { get; }

        protected virtual Point LastPoint { get; set; }

        internal protected virtual PartialReadOnlyDictionary<NamedValue<int>, DrawingBase, int> DrawingStyles { get; }

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