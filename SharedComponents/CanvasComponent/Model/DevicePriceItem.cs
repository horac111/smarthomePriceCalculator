using CanvasComponent.Abstract;
using System.Text.Json.Serialization;

namespace CanvasComponent.Model
{
    public class DevicePriceItem
    {

        public DevicePriceItem(ISmartDevice device, double price)
        {
            Id = device.Id;
            Name = device.Name;
            Price = price;
            Count = 1;
        }

        [JsonConstructor]
        internal DevicePriceItem(int id, string name, double price, int count)
        {
            Id = id;
            Name = name;
            Price = price;
            Count = count;
        }

        public int Id { get; init; }
        public string Name { get; init; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
