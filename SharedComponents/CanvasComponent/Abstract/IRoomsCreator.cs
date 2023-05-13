using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    public interface IRoomsCreator
    {
        IEnumerable<Line> AllLines { get; }

        void UpdateFromLines(IEnumerable<Line> lines);
    }
}