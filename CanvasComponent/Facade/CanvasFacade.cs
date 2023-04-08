using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using CanvasComponent.Service;
using Excubo.Blazor.Canvas;
using Excubo.Blazor.Canvas.Contexts;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.Facade
{
    public class CanvasFacade : NotifyPropertyChanged, IDisposable, ICanvasFacade
    {
        private DrawByStyleBase drawByStyle;

        private RoomsCreatorBase roomsCreator;

        private Service.Drawing drawing;


        private Project project;

        public IDrawingHelper DrawingHelper { get; set; }

        public int SelectedStyle
        {
            get => drawByStyle.SelectedDrawingStyle;
            set => drawByStyle.SelectedDrawingStyle = value;
        }
        public double GridSize
        {
            get => drawByStyle.GridSize;
            set => drawByStyle.GridSize = value;
        }
        public bool SnapToGrid
        {
            get => drawByStyle.SnapToGrid;
            set => drawByStyle.SnapToGrid = value;
        }
        public IEnumerable<INamedValue<int>> DrawingTypes
        {
            get => drawByStyle.DrawingStyles.Keys;
        }
        public double AutoComplete
        {
            get => roomsCreator.AutoComplete;
            set => roomsCreator.AutoComplete = value;
        }
        public IEnumerable<Room> Rooms
        {
            get => project.Rooms;
        }
        public double Thickness
        {
            get => drawing.Thickness;
            set => drawing.Thickness = value;
        }
        public bool ShowGrid
        {
            get => drawing.ShowGrid;
            set => drawing.ShowGrid = value;
        }

        public bool CanStepForward
        {
            get => project.CanStepForward;
        }

        public bool CanStepBackward
        {
            get => project.CanStepBackward;
        }

        public IEnumerable<ISmartDevice> Devices => project.Devices;

        private DragAndDropService<ISmartDevice> dragAndDrop { get; init; }

        public double TotalPrice => project.TotalPrice;

        public Canvas Canvas
        {
            get => drawing.Canvas;
            set => drawing.Canvas = value;
        }

        public event EventHandler<RoomsEventArgs> NewRooms
        {
            add => roomsCreator.RoomsFound += value;
            remove => roomsCreator.RoomsFound -= value;
        }

        public NewRoomDelegate NewRoom
        {
            get => drawing.NewRoom;
            set => drawing.NewRoom = value;
        }

        public event EventHandler<StepEventArgs> Step;


        public CanvasFacade(Project project, Service.Drawing drawing)
            : this(new DrawByStyle(), new RoomsCreator(), drawing, project)
        {


        }
        public CanvasFacade(DrawByStyleBase byStyle, RoomsCreatorBase roomsCreator,
            Service.Drawing drawing, Project project)
        {
            if (byStyle is null)
                throw new ArgumentNullException(nameof(byStyle));
            if (roomsCreator is null)
                throw new ArgumentNullException(nameof(roomsCreator));
            if (drawing is null)
                throw new ArgumentNullException(nameof(drawing));
            if (project is null)
                throw new ArgumentNullException(nameof(project));

            drawByStyle = byStyle;
            this.roomsCreator = roomsCreator;
            this.drawing = drawing;
            dragAndDrop = new();
            this.project = project;
            drawByStyle.Draw += DrawCalled;
            drawByStyle.NewLines += roomsCreator.NewLines;
            roomsCreator.RoomsFound += project.OnRoomsFound;
            project.NewRooms += drawing.OnNewRooms;
            drawByStyle.PropertyChanged += OnPropertyChanged;
            drawing.PropertyChanged += OnPropertyChanged;
            Step += project.OnStep;
            Step += roomsCreator.OnStep;
        }

        public async Task OnAfterRender(bool firstTime)
        {
            if (firstTime)
            {
                await drawing.Initialize(DrawingHelper, project, drawByStyle, roomsCreator);
                project.Initialize(DrawingHelper);
            }
            await drawing.Draw(0);
        }



        private async void DrawCalled(object sender, EventArgs e)
            => await drawing.Draw();

        public void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == 0)
                drawByStyle.MouseDown(DrawingHelper.TransformEventArgs(e));
            else
                drawByStyle.Clear();
        }

        public void OnMouseMove(MouseEventArgs e)
            => drawByStyle.MouseMove(DrawingHelper.TransformEventArgs(e));

        public void OnDragStart(ISmartDevice device, string zone)
            => dragAndDrop.OnDragStart(device, zone);

        public void Dispose()
        {
            drawing?.Dispose();
            drawByStyle.Draw -= DrawCalled;
            drawByStyle.NewLines -= roomsCreator.NewLines;
            roomsCreator.RoomsFound -= project.OnRoomsFound;
            project.NewRooms -= drawing.OnNewRooms;
            drawByStyle.PropertyChanged -= OnPropertyChanged;
            drawing.PropertyChanged -= OnPropertyChanged;
        }

        public void OnDragDrop(DragEventArgs e, string zone)
            => project.OnDragDrop(e, dragAndDrop.OnDrop(zone));

        public void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                drawByStyle.Clear();
        }

        public void OnStep(bool forward)
             => Step.Invoke(this, new(forward));
    }
}
