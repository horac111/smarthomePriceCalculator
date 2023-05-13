﻿using CanvasComponent.Abstract;
using CanvasComponent.DrawingStyles;
using CanvasComponent.Enums;
using CanvasComponent.Extensions;
using CanvasComponent.Model;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Service
{
    public class DrawByStyle : DrawByStyleBase
    {
        private int selectedDrawingStyle = 2;

        internal protected override int SelectedDrawingStyle
        {
            get => selectedDrawingStyle;
            set
            {
                CurrentStyle.NewLines -= OnNewLines;
                CurrentStyle.ContinueDrawing = false;
                selectedDrawingStyle = value;
                CurrentStyle.NewLines += OnNewLines;
                LastPoint = default;
                Lines = new List<Line>();
                OnDraw();
            }
        }

        protected override DrawingBase CurrentStyle { get => drawingStyles[selectedDrawingStyle]; }


        public DrawByStyle()
        {
            CurrentStyle.NewLines += OnNewLines;
        }

        private NamedDictionary<INamedValue<int>, DrawingBase, int> drawingStyles = new()
        {
            { new NamedValue<int>("Draw as straight line", (int) DrawingStyle.StraightLine), new StraightLine() },
            { new NamedValue<int>("Draw as polygon", (int) DrawingStyle.Polygon), new Polygon() },
            { new NamedValue<int>("Draw as Rectangle", (int)DrawingStyle.Rectangle), new Rectangle() },
        };

        protected internal override IDictionary<INamedValue<int>, DrawingBase> DrawingStyles { get => drawingStyles; }

        public override void MouseDown(MouseEventArgs e)
        {
            Point point = SnapedPoint(e);
            if(DeleteMode)
            {
                OnDelete(this, new(DeleteRange, point));
                return;
            }

            if (LastPoint is not null)
            {
                CurrentStyle.OnMouseDown(LastPoint.Value, point);
                if (!CurrentStyle.ContinueDrawing)
                {
                    Lines = new List<Line>();
                    LastPoint = null;
                }
                else
                {
                    Lines = CurrentStyle.OnMouseMove(LastPoint.Value, point);
                    LastPoint = point;
                }
            }
            else
            {
                LastPoint = point;
                CurrentStyle.ContinueDrawing = true;
            }
        }

        public override void MouseMove(MouseEventArgs e)
        {
            if (LastPoint is not null && CurrentStyle.ContinueDrawing)
            {
                Point point = SnapedPoint(e);
                Lines = CurrentStyle.OnMouseMove(LastPoint.Value, point);
                OnDraw();
            }
        }

        private Point SnapedPoint(MouseEventArgs e)
        {
            if (SnapToGrid)
                return new(ClosestMultiplier(e.ClientX), ClosestMultiplier(e.ClientY));

            return new(e.ClientX, e.ClientY);
        }

        private double ClosestMultiplier(double number)
            => number - (number % GridDensity > GridDensity / 2 ?
                number % GridDensity - GridDensity :
                number % GridDensity);

        public override void Clear()
        {
            LastPoint = default;
            CurrentStyle.ContinueDrawing = false;
            Lines = new List<Line>();
            OnDraw();
        }
    }
}