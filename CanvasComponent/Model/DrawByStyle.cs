﻿using CanvasComponent.Abstract;
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
    class DrawByStyle : DrawByStyleBase
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

        internal protected override DrawingBase CurrentStyle { get => DrawingStyles.GetChangable(selectedDrawingStyle); }


        public DrawByStyle()
        {
            CurrentStyle.NewLines += OnNewLines;
        }

        internal protected override 
            PartialReadOnlyDictionary<NamedValue<int>, DrawingBase, int> DrawingStyles { get; } = new(3)
            {
                { new("Draw as straight line", (int)DrawingStyle.StraightLine), new StraightLine() },
                { new("Draw as polygon", (int)DrawingStyle.Polygon), new Polygon() },
                { new("Draw as Rectangle", (int)DrawingStyle.Rectangle), new Rectangle() },
            };
        
        public override void MouseDown(MouseEventArgs e)
        {
            Point point = snapedPoint(e);
            if (LastPoint != default)
            {
                CurrentStyle.OnMouseDown(LastPoint, point);
                if (!CurrentStyle.ContinueDrawing)
                {
                    Lines = new List<Line>();
                    LastPoint = default;
                }
                else
                {
                    Lines = CurrentStyle.OnMouseMove(LastPoint, point).Concat(Lines);
                    LastPoint = point;
                }
                GC.Collect();
            }
            else
            {
                LastPoint = point;
                CurrentStyle.ContinueDrawing = true;
            }
        }

        public override void MouseMove(MouseEventArgs e)
        {
            if (CurrentStyle.ContinueDrawing)
            {
                Point point = snapedPoint(e);
                Lines = Lines.ReplaceLasts(CurrentStyle.OnMouseMove(LastPoint, point));
                OnDraw();
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

        public override void Clear()
        {
            LastPoint = default;
            CurrentStyle.ContinueDrawing = false;
            Lines = new List<Line>();
            OnDraw();
        }
    }
}
