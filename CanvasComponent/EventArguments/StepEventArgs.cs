using System;

namespace CanvasComponent.EventArguments
{
    public class StepEventArgs : EventArgs
    {
        public StepEventArgs(bool forward)
        {
            Forward = forward;
        }

        public bool Forward { get; set; }
    }
}
