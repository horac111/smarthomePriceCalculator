using CanvasComponent.Abstract;
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
    internal class IDrawingHelperConverter : JsonConverter<IDrawingHelper>
    {
        public override IDrawingHelper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double left = 0, top = 0, height = 0, width = 0, pixelsToMeter = 50;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new FormatException("Expected property name");

                var propertyName = reader.GetString();
                reader.Read();
                if (propertyName == nameof(IDrawingHelper.Top))
                    top = reader.GetDouble();
                else if (propertyName == nameof(IDrawingHelper.Left))
                    left = reader.GetDouble();
                else if (propertyName == nameof(IDrawingHelper.Height))
                    height = reader.GetDouble();
                else if (propertyName == nameof(IDrawingHelper.Width))
                    width = reader.GetDouble();
                else if (propertyName == nameof(IDrawingHelper.PixelsToMeter))
                    pixelsToMeter = reader.GetDouble();
                else
                    throw new FormatException("Unknown format");
            }
            return new DrawingHelper(width, height, top, left)
            {
                PixelsToMeter = pixelsToMeter
            };
        }

        public override void Write(Utf8JsonWriter writer, IDrawingHelper value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber(nameof(IDrawingHelper.Left), value.Left);
            writer.WriteNumber(nameof(IDrawingHelper.PixelsToMeter), value.PixelsToMeter);
            writer.WriteNumber(nameof(IDrawingHelper.Height), value.Height);
            writer.WriteNumber(nameof(IDrawingHelper.Width), value.Width);
            writer.WriteNumber(nameof(IDrawingHelper.Top), value.Top);
            writer.WriteEndObject();
        }
    }
}
