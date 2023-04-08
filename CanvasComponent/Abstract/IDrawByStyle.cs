using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    public interface IDrawByStyleBase
    {
        double GridSize { get; set; }
        IEnumerable<Line> Lines { get; set; }

        void Clear();
    }
}