using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Extensions;
using CanvasComponent.Model;
using CanvasComponent.Model.SmartDevice;
using CanvasComponent.Service;
using Excubo.Blazor.Canvas;
using Excubo.Blazor.Canvas.Contexts;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.ViewModel
{
    public class CanvasViewModel : NotifyPropertyChanged, IDisposable, ICanvasViewModel
    {
        private DrawByStyle drawing = new();

        private RoomsCreator roomsCreator = new();

        public IDrawingHelper DrawingHelper { get; set; }

        public int SelectedStyle
        {
            get => drawing.SelectedDrawingStyle;
            set => ChangeAndRaise(x => x.SelectedDrawingStyle, drawing, value);
        }
        public double GridSize
        {
            get => drawing.GridSize;
            set => ChangeAndRaise(x => x.GridSize, drawing, value);
        }
        public bool SnapToGrid
        {
            get => drawing.SnapToGrid;
            set => ChangeAndRaise(x => x.SnapToGrid, drawing, value);
        }
        public IEnumerable<INamedValued<int>> DrawingTypes
        {
            get => drawing.DrawingStyles.Keys;
        }
        public double AutoComplete
        {
            get => roomsCreator.AutoComplete;
            set => ChangeAndRaise(x => x.AutoComplete, roomsCreator, value);
        }
        public List<Room> Rooms
        {
            get => roomsCreator.Rooms;
        }

        public IEnumerable<ISmartDevice> Devices { get; init; }

        private DragAndDropService<ISmartDevice> dragAndDrop { get; init; }

        public double TotalPrice { get => 
                Rooms.Sum(x => x.Devices.Sum(y => y.DeterminPrice(DrawingHelper.ToMetersSquared(x.Size)))); }

        public bool ShowGrid { get; set; } = true;

        public Canvas Canvas { get; set; }
        private Context2D ctx;
        public double Thickness { get; set; } = 1;


        public event EventHandler<NewRoomsEventArgs> NewRooms
        {
            add => roomsCreator.NewRooms += value;
            remove => roomsCreator.NewRooms -= value;
        }

        public NewRoomDelegate NewRoom { get; set; }

        private async void onNewRooms(object sender, NewRoomsEventArgs e)
        {
            await clearCanvas(ctx.CreateBatch());
            drawing.currentStyle.ContinueDrawing = false;
            foreach (var room in e.Rooms)
            {
                var batch = ctx.CreateBatch();
                await clearCanvas(batch);
                await drawRoom(room, batch, color: "Blue");
                await batch.SaveAsync();
                await batch.DisposeAsync();
                    
                if (NewRoom is not null)
                    await NewRoom(this, new(room));
            }
            draw(null, null);
        }

        public CanvasViewModel()
        {
            drawing.Draw += draw;
            drawing.NewLines += roomsCreator.NewLines;
            roomsCreator.NewRooms += onNewRooms;
            drawing.GridSize = 50;
            var icon = "data:image/png;base64," +
                Convert.ToBase64String(File.ReadAllBytes(Path.Combine(Directory.GetCurrentDirectory(), "idea.png")));
            Devices = new List<ISmartDevice>()
            { 
                new PricePerMeterSmartDevice()
                {
                    Id = 0,
                    Price = 10,
                    Icon = icon,
                },
                new DevicesPerMeterSmartDevice()
                {
                    Id = 1,
                    Price = 10,
                    DevicesPerMeter = 3,
                    Icon = icon,
                },
                new DevicesPerRoomSmartDevice()
                {
                    Id = 10,
                    Price = 10,
                    DevicesInRoom = 2,
                    Icon = icon,
                }
            };
            dragAndDrop = new();
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
            }

        }

        private async void draw(object sender, EventArgs e)
        {
            Batch2D batch = ctx.CreateBatch();
            try
            {
                await batch.LineWidthAsync(Thickness);

                await clearCanvas(batch);

                foreach (var line in roomsCreator.LinesWithoutRoom)
                    await drawLine(line, "Black", batch);

                foreach (var line in drawing.Lines)
                    await drawLine(line, "Red", batch, true);

                foreach (var room in Rooms)
                        await drawRoom(room, batch);
                
                await batch.LineWidthAsync(0.5);
                if (ShowGrid)
                    await addGrid(batch);
            }
            finally
            {
                await batch.DisposeAsync();
            }
        }

        private async Task drawRoom(Room room, Batch2D batch = null, string color = "Purple")
        {
            var disposableBatch = batch ??  ctx.CreateBatch();
            try
            {
                foreach (var line in room.Lines)
                    await drawLine(line, color, disposableBatch, true);

                /*Line cross = new(new(room.Center.X - 5, room.Center.Y - 5), new(room.Center.X + 5, room.Center.Y + 5));
                await drawLine(cross, "Blue", disposableBatch);
                cross = new(new(room.Center.X + 5, room.Center.Y - 5), new(room.Center.X - 5, room.Center.Y + 5));
                await drawLine(cross, "Blue", disposableBatch);*/
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
                await batch.StrokeTextAsync($"{Math.Round( DrawingHelper.ToMeters(line.Length()), 2)} m",
                    ((line.Start.X + line.End.X) / 2) + 5, ((line.Start.Y + line.End.Y) / 2) + 10);
            await batch.StrokeAsync();
        }

        private async Task clearCanvas(Batch2D batch)
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
            if(e.Button == 0)
                drawing.MouseDown(DrawingHelper.TransformEventArgs(e));
        }

        public void OnMouseMove(MouseEventArgs e)
            => drawing.MouseMove(DrawingHelper.TransformEventArgs(e));

        public void OnDragStart(ISmartDevice device, string zone)
            => dragAndDrop.OnDragStart(device, zone);

        public void Dispose()
        {
            ctx.DisposeAsync();
        }

        public void OnDragDrop(DragEventArgs e, string zone)
        {
            e = DrawingHelper.TransformEventArgs(e);
            var droppedPoint = new Point(e.ClientX, e.ClientY);
            var room = Rooms.FirstOrDefault(x => x.Contains(droppedPoint));
            var currentDragged = dragAndDrop.OnDrop(zone);
            if (room is not null && currentDragged != default)
            {
                room.Devices.Add(currentDragged);
                OnPropertyChanged(nameof(TotalPrice));
            }
            draw(null, null);
        }

        public void OnKeyDown(KeyboardEventArgs e)
        {
            
        }
    }
}
