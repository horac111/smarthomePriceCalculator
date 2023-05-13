namespace CanvasComponent.Abstract
{
    public interface INamedValue<T>
    {
        T Value { get; }

        string Name { get; set; }
    }
}
