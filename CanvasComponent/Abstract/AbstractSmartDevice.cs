using System;

namespace CanvasComponent.Abstract
{
    public abstract class AbstractSmartDevice : ISmartDevice
    {
        public int Id { get; init; }

        public string Icon { get; init; }
        public double Price { get; init; }
        public double BasePrice { get; init; }

        public virtual double DeterminPrice(double size)
            => BasePrice;

        public event EventHandler Drop;

        protected internal virtual void OnDrop()
            => Drop?.Invoke(this, EventArgs.Empty);

        void IDragable.OnDrop()
            => OnDrop();
    }
}
