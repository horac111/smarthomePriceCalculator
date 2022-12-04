using CanvasComponent.EventArguments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Model
{
    public abstract class RoomsCreatorBase
    {
        protected virtual IList<Line> AllLines { get; set; } = new List<Line>();

        internal protected virtual ICollection<Room> Rooms { get; private set; } = new List<Room>();

        internal IEnumerable<Line> LinesWithoutRoom { get => AllLines.Except(Rooms.SelectMany(x => x.Lines)); }

        internal protected virtual double AutoComplete { get; set; } = 5;

        public event EventHandler<NewRoomsEventArgs> NewRooms;

        protected internal abstract void NewLines(object sender, NewLinesEventArgs e);

        protected virtual void OnNewRooms(IEnumerable<Room> rooms)
            => NewRooms?.Invoke(this, new(rooms));
    }
}