using CanvasComponent.Abstract;
using CanvasComponent.EventArguments;
using CanvasComponent.Service;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CanvasComponent.Model
{
    public class Project : NotifyPropertyChanged
    {
        private SteppingEnumerable<Room> rooms = new();
        public IEnumerable<Room> Rooms { get => rooms; }

        public bool CanStepForward { get => rooms.CanStepForward(); }
        public bool CanStepBackward { get => rooms.CanStepBackward(); }

        public double TotalPrice { get; private set; }

        public event EventHandler<RoomsEventArgs> NewRooms;

        internal protected IEnumerable<ISmartDevice> Devices { get; private set; }

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

            }
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
            var toAdd = e.Rooms.Except(rooms);
            if (toAdd.Count() != 0)
            {
                rooms.AddRange(toAdd);
                /*OnPropertyChanged(nameof(Rooms));
                OnPropertyChanged(nameof(CanStepForward));
                OnPropertyChanged(nameof(CanStepBackward));*/
                NewRooms.Invoke(this, new(toAdd));
            }
        }

        public void OnStep(object sender, StepEventArgs e)
        {
            rooms.Step(e.Forward);
            OnPropertyChanged(nameof(Rooms));
            OnPropertyChanged(nameof(CanStepForward));
            OnPropertyChanged(nameof(CanStepBackward));
        }
    }
}
