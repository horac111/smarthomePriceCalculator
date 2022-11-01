using CanvasComponent.Abstract;
using CanvasComponent.Drawing;
using CanvasComponent.Enums;
using CanvasComponent.EventArguments;
using CanvasComponent.Extensions;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.Model
{
    class DrawByStyle : NotifyPropertyChanged, IDrawByStyle
    {
        private int selectedDrawingStyle = 2;
        internal protected int SelectedDrawingStyle
        {
            get => selectedDrawingStyle;
            set
            {
                currentStyle.NewLines -= onNewLines;
                currentStyle.ContinueDrawing = false;
                selectedDrawingStyle = value;
                currentStyle.NewLines += onNewLines;
                lastPoint = default;
                Lines = new List<Line>();
                onDraw();
            }
        }

        internal protected bool SnapToGrid { get; set; }

        internal protected double GridSize { get; set; }

        internal protected IEnumerable<Line> Lines { get; set; } = new List<Line>();

        internal DrawingBase currentStyle { get => DrawingStyles.GetChangable(selectedDrawingStyle); }

        protected Point lastPoint;

        public DrawByStyle()
        {
            currentStyle.NewLines += onNewLines;
        }

        internal PartialReadOnlyDictionary<NamedValue<int>, DrawingBase, int> DrawingStyles { get; } = new(3)
        {
            { new("Draw as straight line", (int)DrawingStyle.StraightLine), new StraightLine() },
            { new("Draw as polygon", (int)DrawingStyle.Polygon), new Polygon() },
            { new("Draw as Rectangle", (int)DrawingStyle.Rectangle), new Rectangle() },
        };

        public event EventHandler<NewLinesEventArgs> NewLines;

        public event EventHandler Draw;

        protected void onDraw()
        {
            Draw?.Invoke(this, new());
        }

        protected void onNewLines(object sender, NewLinesEventArgs e)
        {
            NewLines?.Invoke(sender, e);
        }

        public void MouseDown(MouseEventArgs e)
        {
            Point point = snapedPoint(e);
            if (lastPoint != default)
            {
                
                currentStyle.OnMouseDown(lastPoint, point);
                if (!currentStyle.ContinueDrawing)
                {
                    Lines = new List<Line>();
                    lastPoint = default;
                }
                else
                {
                    Lines = currentStyle.OnMouseMove(lastPoint, point).Concat(Lines);
                    lastPoint = point;
                }
                    
            }
            else
            {
                lastPoint = point;
                currentStyle.ContinueDrawing = true;
            }
                

        }

        public void MouseMove(MouseEventArgs e)
        {
            if (currentStyle.ContinueDrawing)
            {
                Point point = snapedPoint(e);
                Lines = Lines.ReplaceLasts(currentStyle.OnMouseMove(lastPoint, point));
                onDraw();
            }
        }

        private Point snapedPoint(MouseEventArgs e)
        {
            if (SnapToGrid)
                return new(closestMultiplier(e.ClientX), closestMultiplier(e.ClientY));

            return new(e.ClientX, e.ClientY);
        }

        private double closestMultiplier(double number)
            => number - ((number % GridSize > GridSize / 2) ? (number % GridSize - GridSize)  : number % GridSize);
    }
}
