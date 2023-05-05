using CanvasComponent.Abstract;

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

        public int Id { get; init; }
        public string Name { get; init; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
