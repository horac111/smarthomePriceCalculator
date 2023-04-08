using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.Abstract
{
    public interface IRoomsCreatorBase
    {
        IEnumerable<Line> AllLines { get; }
    }
}