using CanvasComponent.Model;
using System;
using System.Collections.Generic;

namespace CanvasComponent.EventArguments
{
    public class RoomsEventArgs : EventArgs
    {
        public IEnumerable<Room> Rooms { get; }

        public RoomsEventArgs(IEnumerable<Room> r) => Rooms = r;
    }
}
