using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class DevicesPerMeterSmartDevice : AbstractSmartDevice
    {

        public double DevicesPerMeter { get; init; }

        public override double DeterminPrice(double size, double wiring)
            => Math.Round(base.DeterminPrice(size, wiring) + ((int)(size * DevicesPerMeter) +
                ((size % DevicesPerMeter == 0) ? 0 : 1)) * Price
                , ISmartDevice.PriceRounding);
    }
}
