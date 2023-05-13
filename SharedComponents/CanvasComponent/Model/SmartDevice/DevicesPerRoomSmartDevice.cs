using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class DevicesPerRoomSmartDevice : AbstractSmartDevice
    {

        public int DevicesInRoom { get; init; }
        public override double DeterminPrice(double size, double wiring)
         => Math.Round(base.DeterminPrice(size, wiring) + DevicesInRoom * Price, ISmartDevice.PriceRounding);
    }
}
