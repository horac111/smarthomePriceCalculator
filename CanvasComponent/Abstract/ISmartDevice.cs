namespace CanvasComponent.Abstract
{
    public interface ISmartDevice : IDragable
    {
        public static int PriceRounding { get; set; } = 2;
        int Id { get; }
        string Icon { get; init; }
        double Price { get; init; }

        double BasePrice { get; init; }

        double DeterminPrice(double size);
    }
}