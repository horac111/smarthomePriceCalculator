using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Service;
using Microsoft.AspNetCore.Components.Web;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasComponent.Model
{
    /// <summary>
    /// Contains rooms and calculates the total price
    /// </summary>
    public class Project : NotifyPropertyChanged, INamed
    {
        public string Name { get; set; }

        private SteppingEnumerable<Room> rooms = new();
        public IEnumerable<Room> Rooms { get => rooms; }

        public bool CanStepForward { get => rooms.CanStepForward(); }
        public bool CanStepBackward { get => rooms.CanStepBackward(); }

        public double TotalPrice { get; private set; }

        public event EventHandler<RoomsEventArgs> NewRooms;

        internal protected IEnumerable<ISmartDevice> Devices { get; }

        private Room ContainsCentralUnit { get; set; }
        private IDrawingHelper drawingHelper;

        public void Initialize(IDrawingHelper helper)
        {
            this.drawingHelper = helper;
        }

        protected virtual void CalculateTotalPrice()
        {
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
                    price += device.DeterminPrice(drawingHelper.ToMetersSquared(room.Size), wiring);
                }
            }
            TotalPrice = price;
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
                CalculateTotalPrice();
                if (currentDragged.IsCentralUnit)
                {
                    ContainsCentralUnit = room;
                    foreach (var device in Devices)
                        device.IsVisible = !device.IsCentralUnit;
                }
                OnPropertyChanged(nameof(rooms));
            }
        }

        [JsonConstructor]
        private Project(IEnumerable<Room> rooms, string name, double totalPrice) 
        {
            this.rooms = new(rooms);
            Name = name;
            TotalPrice = totalPrice;
        }

        public Project(IEnumerable<ISmartDevice> devices)
        {
            if (devices is null)
                throw new ArgumentNullException(nameof(devices));
            if (devices.GroupBy(x => x.Id).Any(x => x.Count() > 1))
                throw new ArgumentException("Colection of devices contains duplicate ids.");
            Devices = devices;
        }

        public void OnRoomsFound(object sender, RoomsEventArgs e)
        {
            var toAdd = e.Rooms.Except(rooms).ToList();
            rooms.AddRange(toAdd);
            RoomsChanged();
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
                foreach(var device in room.Devices.Where(x => !devicesId.Contains(x.Id)))
                    room.Devices.Remove(device);

            rooms = new(project.Rooms.ToArray());
            Name = project.Name;
            TotalPrice = project.TotalPrice;
            RoomsChanged();
        }

        private void RoomsChanged()
        {
            OnPropertyChanged(nameof(Rooms));
            OnPropertyChanged(nameof(CanStepForward));
            OnPropertyChanged(nameof(CanStepBackward));
        }
    }
}
