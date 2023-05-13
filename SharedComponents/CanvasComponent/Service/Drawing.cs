using CanvasComponent.Abstract;
using CanvasComponent.Converters;
using CanvasComponent.EventArguments;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using Excubo.Blazor.Canvas;
using Excubo.Blazor.Canvas.Contexts;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.Service
{
    /// <summary>
    /// Handles drawing of the components to the canvas
    /// </summary>
    public class Drawing : NotifyPropertyChanged, IDisposable, IDrawing
    {
        public Drawing(IJSRuntime js)
        {
            this.js = js;
        }

        internal Drawing() { }

        #region Private
        private IJSRuntime js;
        private string noRoomColor;
        private string roomColor;
        private string temporaryColor;
        private string gridColor;
        private SemaphoreSlim drawing = new(1, 1);
        private Context2D ctx;
        private IDrawByStyle drawByStyle;
        private Stopwatch lastColorChange = new();
        private readonly TimeSpan colorChangingFrequency = TimeSpan.FromSeconds(1);

        private IRoomsCreator roomsCreator;
        private IDrawingHelper drawingHelper;
        private Project project;
        #endregion

        #region Public
        public Canvas Canvas { get; set; }

        public double Thickness { get; set; } = 1;
        public bool ShowGrid { get; set; }

        public NewRoomDelegate NewRoom { get; set; }
        #endregion

        #region Methods
        public async void OnNewRooms(object sender, RoomsEventArgs e)
        {
            if (!await drawing.WaitAsync(1000))
                return;
            try
            {
                drawByStyle.Clear();
                foreach (var room in e.Rooms)
                {
                    var batch = ctx.CreateBatch();
                    await ClearCanvas(batch);
                    await DrawRoom(room, roomColor, batch);
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
            await Draw(1000);
        }

        private async Task ClearCanvas(Batch2D batch)
        {
            await batch.ClearRectAsync(0, 0, drawingHelper.Width, drawingHelper.Height);
        }

        private async Task AddGrid(Batch2D batch)
        {
            for (double i = 0; i < drawingHelper.Width; i += drawByStyle.GridDensity)
                await DrawLine(new(new(i, 0), new(i, drawingHelper.Height)), gridColor, batch);
            for (double i = 0; i < drawingHelper.Height; i += drawByStyle.GridDensity)
                await DrawLine(new(new(0, i), new(drawingHelper.Width, i)), gridColor, batch);
        }

        private async Task DrawRoom(Room room, string color, Batch2D batch = null)
        {
            var disposableBatch = batch ?? ctx.CreateBatch();
            try
            {
                foreach (var line in room.Lines)
                    await DrawLine(line, color, disposableBatch, true);

                var center = room.Center;
                if (!string.IsNullOrEmpty(room.Name))
                    await disposableBatch.StrokeTextAsync(room.Name, center.X - 50, center.Y - 20);
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

        private async Task DrawLine(Line line, string color, Batch2D batch, bool showLength = false)
        {
            await batch.StrokeStyleAsync(color);
            await batch.BeginPathAsync();
            await batch.MoveToAsync(line.Start.X, line.Start.Y);
            await batch.LineToAsync(line.End.X, line.End.Y);
            if (showLength)
                await batch.StrokeTextAsync($"{Math.Round(drawingHelper.ToMeters(line.Length()), 2)} m",
                    ((line.Start.X + line.End.X) / 2) + 5, ((line.Start.Y + line.End.Y) / 2) + 10);
            await batch.StrokeAsync();
        }

        public async Task Draw(int wait = 0)
        {
            if (ctx is null || drawingHelper is null)
                return;

            if (!await drawing.WaitAsync(wait))
                return;

            Batch2D batch = null;
            try
            {
                await GetColors();

                batch = ctx.CreateBatch();
                await batch.LineWidthAsync(Thickness);

                await ClearCanvas(batch);
                await js.InvokeVoidAsync("eval", "console.log(\"started fucking drawing\")");
                foreach (var line in roomsCreator.AllLines)
                    await DrawLine(line, noRoomColor, batch);

                foreach (var line in drawByStyle.Lines)
                    await DrawLine(line, temporaryColor, batch, true);

                foreach (var room in project.Rooms)
                    await DrawRoom(room, roomColor, batch);

                if (ShowGrid)
                    await AddGrid(batch);
            }
            finally
            {
                if (batch is not null)
                    await batch.DisposeAsync();
                drawing.Release();
            }
        }

        public async Task Initialize(IDrawingHelper drawingHelper, Project project,
            IDrawByStyle drawByStyle, IRoomsCreator roomsCreator)
        {
            ctx = await Canvas.GetContext2DAsync();
            drawing = new(1, 1);
            await ctx.LineJoinAsync(LineJoin.Round);
            await ctx.LineCapAsync(LineCap.Round);
            this.drawingHelper = drawingHelper;
            this.drawByStyle = drawByStyle;
            this.roomsCreator = roomsCreator;
            this.project = project;
            await Draw(1000);
        }

        public async ValueTask<string> GetCanvasAsImage()
        {
            string id = "img_" + Guid.NewGuid().ToString().Replace('-', '_');

            var assignment = $"window.{id} = document.querySelector(\"#{CanvasFacade.CanvasID}\")" +
                $".toDataURL('image/png').replace(\"image/png\", \"image/octet-stream\");";
            await js.InvokeVoidAsync("eval", assignment);

            return id;
        }

        private async Task GetColors()
        {
            if (!lastColorChange.IsRunning || lastColorChange.Elapsed > colorChangingFrequency)
            {
                List<Task> tasks = new();
                lastColorChange.Restart();
                var func = "getComputedStyle(document.getElementById(\"CanvasContainer\")).getPropertyValue('{0}')";
                tasks.Add(js.InvokeAsync<string>("eval", string.Format(func, "--accent-fill-hover"))
                    .AsTask().ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            temporaryColor = task.Result;
                    }));
                tasks.Add(js.InvokeAsync<string>("eval", string.Format(func, "--neutral-stroke-strong-active"))
                    .AsTask().ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            roomColor = task.Result;
                    }));
                tasks.Add(js.InvokeAsync<string>("eval", string.Format(func, "--neutral-stroke-strong-rest"))
                    .AsTask().ContinueWith(task =>
                    {
                        if (task.IsCompletedSuccessfully)
                            noRoomColor = task.Result;
                    }));
                tasks.Add(js.InvokeAsync<string>("eval", string.Format(func, "--accent-foreground-hover"))
                   .AsTask().ContinueWith(task =>
                   {
                       if (task.IsCompletedSuccessfully)
                           gridColor = task.Result;
                   }));
                await Task.WhenAll(tasks);
            }
        }


        public void Dispose()
        {
            ctx?.DisposeAsync();
        }
        #endregion
    }
}
