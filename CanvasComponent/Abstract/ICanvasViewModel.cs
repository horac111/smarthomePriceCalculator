using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using Excubo.Blazor.Canvas;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    public delegate Task NewRoomDelegate(object sender, NewRoomEventArgs e);
    public interface ICanvasViewModel : INotifyPropertyChanged
    {
        double AutoComplete { get; set; }
        Canvas Canvas { get; set; }
        IEnumerable<ISmartDevice> Devices { get; }
        IDrawingHelper DrawingHelper { get; set; }
        IEnumerable<INamedValue<int>> DrawingTypes { get; }
        double GridSize { get; set; }
        IEnumerable<Room> Rooms { get; }
        int SelectedStyle { get; set; }
        bool ShowGrid { get; set; }
        bool SnapToGrid { get; set; }
        double Thickness { get; set; }
        double TotalPrice { get; }

        event EventHandler<NewRoomsEventArgs> NewRooms;
        NewRoomDelegate NewRoom { get; set; }
        void Dispose();
        Task OnAfterRender(bool firstTime);
        Task OnDragDrop(DragEventArgs e, string zone);
        void OnDragStart(ISmartDevice device, string zone);
        void OnMouseDown(MouseEventArgs e);
        void OnMouseMove(MouseEventArgs e);
        void OnKeyDown(KeyboardEventArgs e);
    }
}