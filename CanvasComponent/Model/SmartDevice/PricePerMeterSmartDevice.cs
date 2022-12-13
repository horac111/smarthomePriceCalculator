using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class PricePerMeterSmartDevice : AbstractSmartDevice
    {
        public override double DeterminPrice(double size, double wiring)
         => Math.Round(base.DeterminPrice(size, wiring) + size * Price, ISmartDevice.PriceRounding);
    }
}
