using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model.SmartDevice
{
    public class DevicesPerRoomSmartDevice : AbstractSmartDevice
    {

        public int DevicesInRoom { get; init; }
        public override double DeterminPrice(double size)
         => Math.Round(base.DeterminPrice(size) + DevicesInRoom * Price, ISmartDevice.PriceRounding);
    }
}
