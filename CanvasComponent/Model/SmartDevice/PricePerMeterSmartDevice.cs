using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class PricePerMeterSmartDevice : AbstractSmartDevice
    {
        public override double DeterminPrice(double size)
         => Math.Round(base.DeterminPrice(size) + size * Price, ISmartDevice.PriceRounding);
    }
}
