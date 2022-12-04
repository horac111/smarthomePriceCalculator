using CanvasComponent.Model;
using System;

namespace CanvasComponent.EventArguments
{
    public class NewRoomEventArgs : EventArgs
    {
        public Room Room { get; }

        public NewRoomEventArgs(Room r) => Room = r;
    }
}
