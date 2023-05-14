﻿using CanvasComponent.Abstract;
using CanvasComponent.Converters;
using CanvasComponent.Enums;
using CanvasComponent.EventArguments;
using CanvasComponent.Extensions;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Fast.Components.FluentUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace CanvasComponent.Model
{
    /// <summary>
    /// Contains rooms and calculates the total price
    /// </summary>
    public class Project : NotifyPropertyChanged, INamed
    {
        public string Name { get; set; }

        private SteppingEnumerable<Room> rooms = new();
        [JsonConverter(typeof(ICollectionConvertor<Room, RoomConverter>))]
        public ICollection<Room> Rooms { get => rooms.ToList(); }

        public bool CanStepForward { get => rooms.CanStepForward(); }
        public bool CanStepBackward { get => rooms.CanStepBackward(); }
        public IEnumerable<DevicePriceItem> DevicePrices { get; private set; } = new DevicePriceItem[0];

        public double TotalPrice { get; private set; }

        public event EventHandler<RoomsEventArgs> NewRooms;

        [JsonConverter(typeof(ICollectionConvertor<ISmartDevice, ISmartDeviceConverter>))]
        internal protected ICollection<ISmartDevice> Devices { get; }

        private Room ContainsCentralUnit { get; set; }
        private IDrawingHelper drawingHelper;


        public void Initialize(IDrawingHelper helper)
        {
            this.drawingHelper = helper ?? new DrawingHelper(0, 0, 0, 0);
        }

        protected virtual void CalculateTotalPrice()
        {
            Dictionary<int, DevicePriceItem> devicePrices = new();
            double price = 0;
            foreach (var room in Rooms)
            {
                foreach (var device in room.Devices)
                {
                    double wiring;
                    if ((ContainsCentralUnit is null || room == ContainsCentralUnit))
                        wiring = drawingHelper.ToMeters(room.Lines.Max(x => x.Length()));
                    else
                        wiring = drawingHelper.ToMeters(Math.Abs(room.Center.X - ContainsCentralUnit.Center.X) +
                            Math.Abs(room.Center.Y - ContainsCentralUnit.Center.Y));
                    var devicePrice = device.DeterminPrice(drawingHelper.ToMetersSquared(room.Size), wiring);
                    price += devicePrice;
                    if (devicePrices.ContainsKey(device.Id))
                    {
                        devicePrices[device.Id].Count++;
                        devicePrices[device.Id].Price += devicePrice;
                    }
                    else
                        devicePrices.Add(device.Id, new(device, devicePrice));
                }
            }
            TotalPrice = price;
            DevicePrices = devicePrices.Values;
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(DevicePrices));
        }

        internal protected virtual void OnDragDrop(DragEventArgs e, ISmartDevice currentDragged)
        {
            e = drawingHelper.TransformEventArgs(e);
            var droppedPoint = new Point(e.ClientX, e.ClientY);
            var possibleRooms = Rooms.Where(x => x.Contains(droppedPoint));
            var room = possibleRooms.OrderBy(x => x.Size).FirstOrDefault();
            if (room is not null && currentDragged != default)
            {
                room.Devices.Add(currentDragged);
                if (currentDragged.IsCentralUnit)
                {
                    ContainsCentralUnit = room;
                    foreach (var device in Devices)
                        device.IsVisible = !device.IsCentralUnit;
                }
                RoomsChanged();
            }
        }

        public Project(IEnumerable<ISmartDevice> devices)
        {
            if (devices is null)
                throw new ArgumentNullException(nameof(devices));
            if (devices.GroupBy(x => x.Id).Any(x => x.Count() > 1))
                throw new ArgumentException("Colection of devices contains duplicate ids.");
            Devices = devices.ToList();
        }

        [JsonConstructor]

        internal Project(IEnumerable<Room> rooms, string name, IEnumerable<ISmartDevice> devices)
            : this(devices)
        {
            this.rooms = new(rooms);
            Name = name;
            ContainsCentralUnit = rooms.FirstOrDefault(x => x.Devices.Any(y => y.IsCentralUnit));
            drawingHelper = new DrawingHelper(0, 0, 0, 0);
            RoomsChanged();
        }

        public void OnRoomsFound(object sender, RoomsEventArgs e)
        {
            var toAdd = e.Rooms.Except(rooms).ToList();
            rooms.AddRange(toAdd);
            if (toAdd.Count() != 0)
            {
                NewRooms.Invoke(this, new(toAdd));
            }
        }

        public void OnStep(object sender, StepEventArgs e)
        {
            rooms.Step(e.Forward);
            RoomsChanged();
        }

        internal protected void UpdateFromProject(Project project)
        {
            var devicesId = Devices.Select(x => x.Id).ToHashSet();
            foreach (var room in project.Rooms)
                foreach (var device in room.Devices)
                {
                    if (!devicesId.Contains(device.Id))
                        room.Devices.Remove(device);
                    else if (device.IsCentralUnit)
                        ContainsCentralUnit = room;
                }

            rooms = new(project.Rooms.ToArray());
            Name = project.Name;

            RoomsChanged();
        }

        private void RoomsChanged()
        {
            CalculateTotalPrice();
            OnPropertyChanged(nameof(Project));
        }

        internal protected void OnLinesDeleted(object sender, DeleteEventArgs e)
        {
            var deleteRooms = Rooms.Where(x => x.Lines
                .Any(y => y.DistanceFromPoint(e.Point).NearlyLesserOrEqual(e.Range))).ToArray();
            rooms.AddRange(deleteRooms, OperationType.Delete);
            RoomsChanged();
        }

    }
}
