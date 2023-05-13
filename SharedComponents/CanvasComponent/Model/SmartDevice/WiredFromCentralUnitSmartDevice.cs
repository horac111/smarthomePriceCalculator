using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class WiredFromCentralUnitSmartDevice : AbstractSmartDevice
    {

        public override double DeterminPrice(double size, double wiring)
            => Math.Round(base.DeterminPrice(size, wiring) + Price * wiring, ISmartDevice.PriceRounding);
    }
}
