using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    public interface IDrawByStyle
    {
        double GridDensity { get; set; }
        IEnumerable<Line> Lines { get; set; }

        void Clear();
    }
}