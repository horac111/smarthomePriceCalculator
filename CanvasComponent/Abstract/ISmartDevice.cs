namespace CanvasComponent.Abstract
{
    public interface ISmartDevice : IDragable
    {
        bool IsCentralUnit { get; set; }
        bool IsVisible { get; set; }
        public static int PriceRounding { get; set; } = 2;
        int Id { get; }
        string Icon { get; init; }
        string Name { get; init; }
        double Price { get; init; }

        double BasePrice { get; init; }

        double DeterminPrice(double size, double wiring);
    }
}