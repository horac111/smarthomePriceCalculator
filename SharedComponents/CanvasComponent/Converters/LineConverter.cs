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
    public class LineConverter : JsonConverter<Line>
    {
        public override Line Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();
            var startX = reader.GetDouble();
            reader.Read();
            reader.Read();
            var startY = reader.GetDouble();
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();
            reader.Read();
            var endX = reader.GetDouble();
            reader.Read();
            reader.Read();
            var endY = reader.GetDouble();
            reader.Read();
            reader.Read();
            return new(new(startX, startY), new(endX, endY));
        }

        public override void Write(Utf8JsonWriter writer, Line value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(Line.Start));
            writer.WriteStartObject();
            writer.WriteNumber("X", value.Start.X);
            writer.WriteNumber("Y", value.Start.Y);
            writer.WriteEndObject();
            writer.WritePropertyName(nameof(Line.End));
            writer.WriteStartObject();
            writer.WriteNumber("X", value.End.X);
            writer.WriteNumber("Y", value.End.Y);
            writer.WriteEndObject();
            writer.WriteEndObject();
        }
    }
}
