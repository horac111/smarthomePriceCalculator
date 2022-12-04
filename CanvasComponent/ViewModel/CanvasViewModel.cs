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

namespace CanvasComponent.ViewModel
{
    public class CanvasViewModel : NotifyPropertyChanged, IDisposable, ICanvasViewModel
    {
        private DrawByStyleBase drawByStyle;

        private RoomsCreatorBase roomsCreator;

        private SemaphoreSlim drawing = new(1, 1);

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
            get => roomsCreator.Rooms;
        }

        public IEnumerable<ISmartDevice> Devices { get; init; }

        private DragAndDropService<ISmartDevice> dragAndDrop { get; init; }

        public double TotalPrice
        {
            get =>
                Rooms.Sum(x => x.Devices.Sum(y => y.DeterminPrice(DrawingHelper.ToMetersSquared(x.Size))));
        }

        public bool ShowGrid { get; set; }

        public Canvas Canvas { get; set; }
        private Context2D ctx;
        public double Thickness { get; set; } = 1;


        public event EventHandler<NewRoomsEventArgs> NewRooms
        {
            add => roomsCreator.NewRooms += value;
            remove => roomsCreator.NewRooms -= value;
        }

        public NewRoomDelegate NewRoom { get; set; }

        private async void OnNewRooms(object sender, NewRoomsEventArgs e)
        {
            if (!await drawing.WaitAsync(1000))
                return;
            try
            {
                drawByStyle.CurrentStyle.ContinueDrawing = false;
                foreach (var room in e.Rooms)
                {
                    var batch = ctx.CreateBatch();
                    await ClearCanvas(batch);
                    await DrawRoom(room, batch, color: "Blue");
                    await batch.SaveAsync();
                    await batch.DisposeAsync();

                    if (NewRoom is not null)
                        await NewRoom(this, new(room));
                }
            }
            finally
            {
                drawing.Release();
            }
            await Draw();
        }

        public CanvasViewModel(IEnumerable<ISmartDevice> devices) : this(new DrawByStyle(), new RoomsCreator(), devices)
        {


        }
        public CanvasViewModel(DrawByStyleBase byStyle, RoomsCreatorBase roomsCreator, IEnumerable<ISmartDevice> devices)
        {
            if (byStyle is null)
                throw new ArgumentNullException(nameof(byStyle));
            if (roomsCreator is null)
                throw new ArgumentNullException(nameof(roomsCreator));
            if (devices is null)
                throw new ArgumentNullException(nameof(devices));
            if (devices.GroupBy(x => x.Id).Any(x => x.Count() > 1))
                throw new ArgumentException("Colection of devices contains duplicate ids.");

            drawByStyle = byStyle;
            this.roomsCreator = roomsCreator;
            drawByStyle.Draw += DrawCalled;
            drawByStyle.NewLines += roomsCreator.NewLines;
            roomsCreator.NewRooms += OnNewRooms;
            drawByStyle.GridSize = 50;
            dragAndDrop = new();
            drawByStyle.PropertyChanged += OnPropertyChanged;
            PropertyChanged += DrawCalled;
            Devices = devices;

        }

        public async Task OnAfterRender(bool firstTime)
        {
            if (firstTime)
            {
                ctx = await Canvas.GetContext2DAsync();
                await ctx.LineJoinAsync(LineJoin.Round);
                await ctx.LineCapAsync(LineCap.Round);
                if (DrawingHelper is null)
                    DrawingHelper = new DrawingHelper(0, 0, 0, 0);
                await Draw();
            }

        }

        private async Task Draw()
        {
            if (ctx is null)
                return;

            if (!await drawing.WaitAsync(5))
                return;

            Batch2D batch = null;
            try
            {
                batch = ctx.CreateBatch();
                await batch.LineWidthAsync(Thickness);

                await ClearCanvas(batch);

                foreach (var line in roomsCreator.LinesWithoutRoom)
                    await drawLine(line, "Black", batch);

                foreach (var line in drawByStyle.Lines)
                    await drawLine(line, "Red", batch, true);

                foreach (var room in Rooms)
                    await DrawRoom(room, batch);

                if (ShowGrid)
                    await addGrid(batch);
            }
            finally
            {
                if (batch is not null)
                    await batch.DisposeAsync();
                drawing.Release();
            }
        }

        private async void DrawCalled(object sender, EventArgs e)
            => await Draw();

        private async Task DrawRoom(Room room, Batch2D batch = null, string color = "Purple")
        {
            var disposableBatch = batch ?? ctx.CreateBatch();
            try
            {
                foreach (var line in room.Lines)
                    await drawLine(line, color, disposableBatch, true);

                Line cross = new(new(room.Center.X - 5, room.Center.Y - 5), new(room.Center.X + 5, room.Center.Y + 5));
                await drawLine(cross, "Blue", disposableBatch);
                cross = new(new(room.Center.X + 5, room.Center.Y - 5), new(room.Center.X - 5, room.Center.Y + 5));
                await drawLine(cross, "Blue", disposableBatch);
                var dict = Devices.ToDictionary(x => x.Id, x => x.Icon);
                var center = room.Center;
                if (!string.IsNullOrEmpty(room.Name))
                    await disposableBatch.StrokeTextAsync(room.Name, center.X, center.Y);
                center = new(center.X - (room.Devices.Count / 2 * 33), center.Y + 20);
                foreach (var device in room.Devices)
                {
                    await disposableBatch.DrawImageAsync($"img{device.Id}", center.X, center.Y, 32, 32);
                    center = center with { X = center.X + 33 };
                }
            }
            finally
            {
                if (batch == null)
                    await disposableBatch.DisposeAsync();
            }
        }

        private async Task drawLine(Line line, string color, Batch2D batch, bool showLength = false)
        {
            await batch.StrokeStyleAsync(color);
            await batch.BeginPathAsync();
            await batch.MoveToAsync(line.Start.X, line.Start.Y);
            await batch.LineToAsync(line.End.X, line.End.Y);
            if (showLength)
                await batch.StrokeTextAsync($"{Math.Round(DrawingHelper.ToMeters(line.Length()), 2)} m",
                    ((line.Start.X + line.End.X) / 2) + 5, ((line.Start.Y + line.End.Y) / 2) + 10);
            await batch.StrokeAsync();
        }

        private async Task ClearCanvas(Batch2D batch)
        {
            await batch.ClearRectAsync(0, 0, DrawingHelper.Width, DrawingHelper.Height);
        }

        private async Task addGrid(Batch2D batch)
        {
            for (double i = 0; i < DrawingHelper.Width; i += GridSize)
                await drawLine(new(new(i, 0), new(i, DrawingHelper.Height)), "Black", batch);
            for (double i = 0; i < DrawingHelper.Height; i += GridSize)
                await drawLine(new(new(0, i), new(DrawingHelper.Width, i)), "Black", batch);
        }

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
            ctx?.DisposeAsync();
        }

        public async Task OnDragDrop(DragEventArgs e, string zone)
        {
            e = DrawingHelper.TransformEventArgs(e);
            var droppedPoint = new Point(e.ClientX, e.ClientY);
            var possibleRooms = Rooms.Where(x => x.Contains(droppedPoint));
            var room = Rooms.FirstOrDefault(x => possibleRooms.All(y => y == x || !y.Contains(x)));
            var currentDragged = dragAndDrop.OnDrop(zone);
            if (room is not null && currentDragged != default)
            {
                room.Devices.Add(currentDragged);
                OnPropertyChanged(nameof(TotalPrice));
            }
            await Draw();
        }

        public void OnKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
                drawByStyle.Clear();
        }
    }
}
