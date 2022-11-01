using CanvasComponent.EventArguments;
using Microsoft.AspNetCore.Components.Web;
using System;

namespace CanvasComponent.Abstract
{
    internal interface IDrawByStyle
    {
        event EventHandler Draw;
        event EventHandler<NewLinesEventArgs> NewLines;

        void MouseDown(MouseEventArgs e);
        void MouseMove(MouseEventArgs e);
    }
}