using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class DevicesPerMeterSmartDevice : AbstractSmartDevice
    {

        public double DevicesPerMeter { get; init; }

        public override double DeterminPrice(double size)
            => Math.Round(base.DeterminPrice(size) + ((int)(size * DevicesPerMeter) + 
                ((size % DevicesPerMeter == 0) ? 0 : 1)) * Price
                , ISmartDevice.PriceRounding);
    }
}
