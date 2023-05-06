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
    public class ProjectConverter : JsonConverter<Project>
    {
        public override Project Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            IEnumerable<Room> rooms = null;
            IEnumerable<ISmartDevice> devices = null;
            string name = "";
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Expected PropertyName token");

                var propName = reader.GetString();

                switch(propName)
                {
                    case nameof(Project.Rooms):
                        rooms = JsonSerializer.Deserialize<List<Room>>(ref reader, options);
                        break;
                    case nameof(Project.Name):
                        reader.Read();
                        name = reader.GetString();
                        break;
                    case nameof(Project.Devices):
                        devices = JsonSerializer.Deserialize<List<ISmartDevice>>(ref reader, options);
                        break;
                };
            }
            return new(rooms, name, devices);
        }

        public override void Write(Utf8JsonWriter writer, Project value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Project.Rooms));
            JsonSerializer.Serialize(writer, value.Rooms, options);
            writer.WritePropertyName(nameof(Project.Name));
            JsonSerializer.Serialize(writer, value.Name, options);
            writer.WritePropertyName(nameof(Project.Devices));
            JsonSerializer.Serialize(writer, value.Devices, options);
            writer.WriteEndObject();
        }

    }
}
