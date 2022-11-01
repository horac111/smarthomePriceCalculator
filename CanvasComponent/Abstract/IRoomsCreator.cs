using CanvasComponent.EventArguments;
using System;

namespace CanvasComponent.Abstract
{
    public interface IRoomsCreator
    {
        event EventHandler<NewRoomsEventArgs> NewRooms;
    }
}