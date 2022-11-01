using Microsoft.AspNetCore.Components.Web;

namespace CanvasComponent.Abstract
{
    public interface IDrawingHelper
    {
        double Height { get; }
        double Left { get; }
        double Top { get; }
        double Width { get; }

        double PixelsToMeter { get; set; } 

        public T TransformEventArgs<T>(T e) where T : MouseEventArgs;

        public double ToMeters(double pixels);
        public double ToMetersSquared(double pixels);

    }
}