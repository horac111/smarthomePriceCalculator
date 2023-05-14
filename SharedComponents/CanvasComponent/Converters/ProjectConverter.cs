using CanvasComponent.Abstract;
using CanvasComponent.Converters;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanvasComponent.Converters
{
    public class ProjectConverter : JsonConverter<Project>
    {
        public override Project Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IEnumerable<Room> rooms = null;
            IEnumerable<ISmartDevice> devices = null;
            JsonSerializerOptions newOptions = new(options);
            newOptions.Converters.Add(new ISmartDeviceConverter());
            newOptions.Converters.Add(new RoomConverter());
            newOptions.Converters.Add(new LineConverter());
            string name = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                var propertyName = reader.GetString().ToLower();

                if (propertyName == nameof(Project.Rooms).ToLower())
                    rooms = JsonSerializer.Deserialize<List<Room>>(ref reader, newOptions);
                else if (propertyName == nameof(Project.Name).ToLower())
                {
                    reader.Read();
                    name = reader.GetString();
                }
                else if (propertyName == nameof(Project.Devices).ToLower())
                    devices = JsonSerializer.Deserialize<List<ISmartDevice>>(ref reader, newOptions);
                else
                    throw new FormatException("Unexpected property name");
            }
            return new(rooms, name, devices);
        }

        public override void Write(Utf8JsonWriter writer, Project value, JsonSerializerOptions options)
        {
            JsonSerializerOptions newOptions = new(options);
            newOptions.Converters.Add(new ISmartDeviceConverter());
            newOptions.Converters.Add(new RoomConverter());
            newOptions.Converters.Add(new LineConverter());
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Project.Rooms).ToLower());
            JsonSerializer.Serialize(writer, value.Rooms, newOptions);
            writer.WritePropertyName(nameof(Project.Name).ToLower());
            JsonSerializer.Serialize(writer, value.Name, newOptions);
            writer.WritePropertyName(nameof(Project.Devices).ToLower());
            JsonSerializer.Serialize(writer, value.Devices, newOptions);
            writer.WriteEndObject();
        }

    }
}
