using CanvasComponent.Abstract;

namespace CanvasComponent.Model.SmartDevice
{
    public class WiredFromCentralUnitSmartDevice : AbstractSmartDevice
    {
        public double PriceForWiring { get; init; }

        public override double DeterminPrice(double size, double wiring)
            => base.DeterminPrice(size, wiring) + PriceForWiring * wiring;
    }
}
