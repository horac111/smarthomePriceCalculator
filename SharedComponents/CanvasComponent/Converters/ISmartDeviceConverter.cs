using CanvasComponent.Abstract;
using CanvasComponent.Model.SmartDevice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanvasComponent.Converters
{
    public class ISmartDeviceConverter : JsonConverter<ISmartDevice>
    {
        public override ISmartDevice Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            var deviceType = reader.GetInt32();
            if (deviceType is < 0 or > 3)
                throw new FormatException("Unknown device type");
            reader.Read();
            reader.Read();
            ISmartDevice device = deviceType switch
            {
                0 => JsonSerializer.Deserialize<DevicesPerMeterSmartDevice>(ref reader, options),
                1 => JsonSerializer.Deserialize<DevicesPerRoomSmartDevice>(ref reader, options),
                2 => JsonSerializer.Deserialize<PricePerMeterSmartDevice>(ref reader, options),
                3 => JsonSerializer.Deserialize<WiredFromCentralUnitSmartDevice>(ref reader, options),
                _ => null
            };
            reader.Read();
            return device;
        }

        public override void Write(Utf8JsonWriter writer, ISmartDevice value, JsonSerializerOptions options)
        {

            int deviceType = value switch
            {
                DevicesPerMeterSmartDevice => 0,
                DevicesPerRoomSmartDevice => 1,
                PricePerMeterSmartDevice => 2,
                WiredFromCentralUnitSmartDevice => 3,
                _ => -1
            };

            writer.WriteStartObject();
            writer.WriteNumber("deviceType", deviceType);
            writer.WritePropertyName("device");

            if (value is DevicesPerMeterSmartDevice perMeter)
                JsonSerializer.Serialize(writer, perMeter, options);
            else if (value is DevicesPerRoomSmartDevice perRoom)
                JsonSerializer.Serialize(writer, perRoom, options);
            else if (value is PricePerMeterSmartDevice pricePerMeter)
                JsonSerializer.Serialize(writer, pricePerMeter, options);
            else if (value is WiredFromCentralUnitSmartDevice fromCentral)
                JsonSerializer.Serialize(writer, fromCentral, options);

            writer.WriteEndObject();
        }
    }
}
