using CanvasComponent.EventArguments;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CanvasComponent.Abstract
{
    /// <summary>
    /// Reacts on new lines and creates rooms
    /// </summary>
    public abstract class RoomsCreatorBase : NotifyPropertyChanged, IRoomsCreator
    {
        public virtual IEnumerable<Line> AllLines { get; }

        internal protected double AutoComplete { get; set; } = 5;

        public event EventHandler<RoomsEventArgs> RoomsFound;
        public event EventHandler<DeleteEventArgs> LinesDeleted;

        protected internal abstract void NewLines(object sender, NewLinesEventArgs e);

        protected virtual void OnRoomsFound(IEnumerable<Room> rooms)
            => RoomsFound?.Invoke(this, new(rooms));

        protected virtual void OnLinesDeleted(DeleteEventArgs e)
            => LinesDeleted?.Invoke(this, e);

        public abstract void OnStep(object sender, StepEventArgs e);
        public abstract void UpdateFromLines(IEnumerable<Line> lines);

        internal protected abstract void OnDelete(object sender, DeleteEventArgs e);
    }
}