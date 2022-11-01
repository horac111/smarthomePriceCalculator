using CanvasComponent.Abstract;

namespace CanvasComponent.Service
{
    public class DragAndDropService<T> where T : IDragable
    {
        private T data;

        private string zone;

        public bool CanDrop(string zone)
            => zone == this.zone;

        public void OnDragStart(T dragging, string zone)
        {
            this.zone = zone;
            data = dragging;
        }

        public T OnDrop(string zone)    
        {
            if (CanDrop(zone))
            {
                data.OnDrop();
                var toReturn = data;
                data = default;
                return toReturn;
            }
            return default;
        }
    }
}
