using CanvasComponent.Abstract;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanvasComponent.Convertors
{
    public class RoomConverter : JsonConverter<Room>
    {
        public override Room Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            var lines = JsonSerializer.Deserialize<IEnumerable<Line>>(ref reader, options);
            reader.Read();
            var devices = JsonSerializer.Deserialize<IEnumerable<ISmartDevice>>(ref reader, options);
            reader.Read();
            if (reader.TokenType == JsonTokenType.EndObject)
                return new(lines, devices, "");
            reader.Read();
            var name = reader.GetString();
            reader.Read();
            return new(lines, devices, name);
        }

        public override void Write(Utf8JsonWriter writer, Room value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Room.Lines));
            JsonSerializer.Serialize(writer, value.Lines, options);
            writer.WritePropertyName(nameof(Room.Devices));
            JsonSerializer.Serialize(writer, value.Devices, options);
            writer.WriteString(nameof(Room.Name), value.Name);
            writer.WriteEndObject();
        }
    }
}
