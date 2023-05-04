using CanvasComponent.Model;
using System;

namespace CanvasComponent.EventArguments
{
    public class DeleteEventArgs : EventArgs
    {
        public DeleteEventArgs()
        {
        }

        public DeleteEventArgs(double range, Point point)
        {
            Range = range;
            Point = point;
        }

        public double Range { get; init; }
        public Point Point { get; init; }
    }
}
