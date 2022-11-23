using CanvasComponent.Model;
using System.Collections.Generic;

namespace CanvasComponent.Extensions
{
    public static class ICollectionExtensions
    {
        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> toAdd)
        {
            if (toAdd is null)
                return;
            foreach(T item in toAdd)
                collection.Add(item);
        }
    }
}
