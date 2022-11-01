using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponent.EventArguments
{
    public class NewRoomsEventArgs : EventArgs
    {
        public IEnumerable<Room> Rooms { get; }

        public NewRoomsEventArgs(IEnumerable<Room> r) => Rooms = r;
    }
}
