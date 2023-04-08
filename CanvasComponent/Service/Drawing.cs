﻿using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using Excubo.Blazor.Canvas;
using Excubo.Blazor.Canvas.Contexts;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.Service
{
    public class Drawing : NotifyPropertyChanged, IDisposable
    {
        public Drawing(IJSRuntime js)
        {
            this.js = js;
            Task.Run(async () =>
            {
                var func = "getComputedStyle(document.getElementById(\"CanvasContainer\")).getPropertyValue('{0}')";
                temporaryColor = await js.InvokeAsync<string>("eval", string.Format(func, "--accent-fill-hover"));
                roomColor = await js.InvokeAsync<string>("eval", string.Format(func, "--neutral-layer-4"));
                noRoomColor = await js.InvokeAsync<string>("eval", string.Format(func, "--neutral-stroke-strong-rest"));
            });
        }

        private IJSRuntime js;
        private string noRoomColor { get; set; }
        private string roomColor { get; set; }
        private string temporaryColor { get; set; }
        private SemaphoreSlim drawing = new(1, 1);
        private Context2D ctx;
        private IDrawByStyle drawByStyle;

        private IRoomsCreatorBase roomsCreator;
        private IDrawingHelper drawingHelper;
        private Project project;

        public Canvas Canvas { get; set; }
        
        public double Thickness { get; set; } = 1;
        public bool ShowGrid { get; set; }

        public NewRoomDelegate NewRoom { get; set; }

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
            for (double i = 0; i < drawingHelper.Width; i += drawByStyle.GridSize)
                await DrawLine(new(new(i, 0), new(i, drawingHelper.Height)), "Black", batch);
            for (double i = 0; i < drawingHelper.Height; i += drawByStyle.GridSize)
                await DrawLine(new(new(0, i), new(drawingHelper.Width, i)), "Black", batch);
        }

        private async Task DrawRoom(Room room, string color, Batch2D batch = null)
        {
            var disposableBatch = batch ?? ctx.CreateBatch();
            try
            {
                foreach (var line in room.Lines)
                    await DrawLine(line, color, disposableBatch, true);

                /*Line cross = new(new(room.Center.X - 5, room.Center.Y - 5), new(room.Center.X + 5, room.Center.Y + 5));
                await drawLine(cross, "Blue", disposableBatch);
                cross = new(new(room.Center.X + 5, room.Center.Y - 5), new(room.Center.X - 5, room.Center.Y + 5));
                await drawLine(cross, "Blue", disposableBatch);*/
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

        public async Task Draw(int wait = 5)
        {
            if (ctx is null)
                return;

            if (!await drawing.WaitAsync(wait))
                return;

            Batch2D batch = null;
            try
            {
                var func = "getComputedStyle(document.getElementById(\"CanvasContainer\")).getPropertyValue('{0}')";
                temporaryColor = await js.InvokeAsync<string>("eval", string.Format(func, "--accent-fill-hover"));
                roomColor = await js.InvokeAsync<string>("eval", string.Format(func, "--neutral-stroke-strong-active"));
                noRoomColor = await js.InvokeAsync<string>("eval", string.Format(func, "--neutral-stroke-strong-rest"));

                batch = ctx.CreateBatch();
                await batch.LineWidthAsync(Thickness);

                await ClearCanvas(batch);

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
                try
                {
                    if (batch is not null)
                        await batch.DisposeAsync();
                }
                catch (TaskCanceledException) { }
                finally
                {
                    drawing.Release();
                }
            }
        }

        public async Task Initialize(IDrawingHelper drawingHelper, Project project,
            IDrawByStyle drawByStyle, IRoomsCreatorBase roomsCreator)
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

        public void Dispose()
        {
            ctx?.DisposeAsync();
        }
    }
}
