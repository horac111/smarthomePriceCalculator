using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.EventArguments
{
    public class NewRoomEventArgs : EventArgs
    {
        public Room Room { get; }

        public NewRoomEventArgs(Room r) => Room = r;
    }
}
