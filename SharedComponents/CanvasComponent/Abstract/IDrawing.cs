using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using Excubo.Blazor.Canvas;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CanvasComponent.Abstract
{
    /// <summary>
    /// Handles drawing of the components to the canvas
    /// </summary>
    public interface IDrawing : INotifyPropertyChanged
    {
        Canvas Canvas { get; set; }
        NewRoomDelegate NewRoom { get; set; }
        bool ShowGrid { get; set; }
        double Thickness { get; set; }

        void Dispose();
        Task Draw(int wait = 5);
        ValueTask<string> GetCanvasAsImage();
        Task<bool> Initialize(IDrawingHelper drawingHelper, Project project, IDrawByStyle drawByStyle, IRoomsCreator roomsCreator);
        void OnNewRooms(object sender, RoomsEventArgs e);
    }
}