using CanvasComponent.Abstract;
using CanvasComponent.Model.JSObjects;
using Microsoft.AspNetCore.Components.Web;

namespace CanvasComponent.Model
{
    internal class DrawingHelper : IDrawingHelper
    {
        public DrawingHelper(double width, double height, double top, double left)
        {
            Width = width;
            Height = height;
            Top = top;
            Left = left;
        }

        public DrawingHelper(BoundingClientRect rect) : this(rect.Width, rect.Height, rect.Top, rect.Left)
        {

        }

        public double Width { get; }
        public double Height { get; }

        public double Top { get; }

        public double Left { get; }
        public double PixelsToMeter { get; set; } = 50;

        public T TransformEventArgs<T>(T e) where T : MouseEventArgs
        {
            e.ClientX -= Left;
            e.ClientY -= Top;
            return e;
        }

        public double ToMeters(double pixels)
         => pixels / PixelsToMeter;

        public double ToMetersSquared(double pixels)
         => pixels / (PixelsToMeter * PixelsToMeter);
    }
}
