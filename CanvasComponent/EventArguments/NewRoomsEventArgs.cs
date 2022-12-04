using CanvasComponent.Model;
using System;
using System.Collections.Generic;

namespace CanvasComponent.EventArguments
{
    public class NewRoomsEventArgs : EventArgs
    {
        public IEnumerable<Room> Rooms { get; }

        public NewRoomsEventArgs(IEnumerable<Room> r) => Rooms = r;
    }
}
