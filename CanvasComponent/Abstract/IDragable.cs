using System;

namespace CanvasComponent.Abstract
{
    public interface IDragable
    {
        public event EventHandler Drop;
        internal void OnDrop();
    }
}
