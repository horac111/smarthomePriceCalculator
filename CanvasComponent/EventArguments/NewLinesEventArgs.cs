using CanvasComponent.Model;
using System;
using System.Collections.Generic;

namespace CanvasComponent.EventArguments
{
    public class NewLinesEventArgs : EventArgs
    {
        public NewLinesEventArgs(IEnumerable<Line> lines)
        {
            Lines = lines;
        }

        public IEnumerable<Line> Lines { get; set; }
    }
}
