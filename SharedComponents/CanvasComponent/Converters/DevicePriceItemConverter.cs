using CanvasComponent.Abstract;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace CanvasComponent.Converters
{
    public class DevicePriceItemConverter : JsonConverter<DevicePriceItem>
    {
        public override DevicePriceItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int id = 0, count = 0;
            double price = 0;
            string name = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new FormatException("Expected property name");

                var propertyName = reader.GetString().ToLower();
                reader.Read();
                if (propertyName == nameof(DevicePriceItem.Id).ToLower())
                    id = reader.GetInt32();
                else if (propertyName == nameof(DevicePriceItem.Name).ToLower())
                    name = reader.GetString();
                else if (propertyName == nameof(DevicePriceItem.Price).ToLower())
                    price = reader.GetDouble();
                else if (propertyName == nameof(DevicePriceItem.Count).ToLower())
                    count = reader.GetInt32();
                else
                    throw new FormatException("Unknown format");
            }
            return new DevicePriceItem(id, name, price, count);
        }

        public override void Write(Utf8JsonWriter writer, DevicePriceItem value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(DevicePriceItem.Id).ToLower(), value.Id);
            writer.WriteString(nameof(DevicePriceItem.Name).ToLower(), value.Name);
            writer.WriteNumber(nameof(DevicePriceItem.Count).ToLower(), value.Count);
            writer.WriteNumber(nameof(DevicePriceItem.Price).ToLower(), value.Price);
            writer.WriteEndObject();
        }
    }
}
