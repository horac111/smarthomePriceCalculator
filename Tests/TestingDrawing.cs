using Excubo.Blazor.Canvas;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CanvasComponentTests
{
    internal class TestingDrawing : IDrawing
    {
        public Canvas Canvas { get; set; }
        public NewRoomDelegate NewRoom { get; set; }
        public bool ShowGrid { get; set; }
        public double Thickness { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {

        }

        public async Task Draw(int wait = 5)
        {
            await Task.FromResult(true);
        }

        public async ValueTask<string> GetCanvasAsImage()
        {
            return await ValueTask.FromResult("");
        }

        public async Task Initialize(IDrawingHelper drawingHelper, Project project, IDrawByStyle drawByStyle, IRoomsCreator roomsCreator)
        {
            await Task.FromResult(true);
        }

        public void OnNewRooms(object sender, RoomsEventArgs e)
        {

        }
    }
}
