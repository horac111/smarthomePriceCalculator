using CanvasComponent.Abstract;
using CanvasComponent.Converters;
using CanvasComponent.Facade;
using CanvasComponent.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CanvasComponent.Converters
{
    public class CanvasFacadeConverter : JsonConverter<CanvasFacade>
    {
        public override CanvasFacade Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonSerializerOptions newOptions = new(options);
            newOptions.Converters.Add(new ProjectConverter());
            newOptions.Converters.Add(new INamedValueConverter());
            newOptions.Converters.Add(new IDrawingHelperConverter());
            CanvasFacade facade = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new FormatException("Expected property name");

                var propertyName = reader.GetString();
                if (propertyName == nameof(CanvasFacade.Project))
                    facade.Project = JsonSerializer.Deserialize<Project>(ref reader, newOptions);
                else if (propertyName == nameof(CanvasFacade.DevicePrices))
                    JsonSerializer.Deserialize<IEnumerable<DevicePriceItem>>(ref reader, newOptions);
                else if (propertyName == nameof(CanvasFacade.DrawingTypes))
                    JsonSerializer.Deserialize<IEnumerable<INamedValue<int>>>(ref reader, newOptions);
                else if (propertyName == nameof(CanvasFacade.DrawingHelper))
                    facade.DrawingHelper = JsonSerializer.Deserialize<IDrawingHelper>(ref reader, newOptions);
                else
                    ReadProperties(propertyName, ref reader, facade);
            }
            if (facade.Project is null)
                throw new FormatException("Project cannot be null");
            if (facade.DrawingHelper is null)
                throw new FormatException("DrawingHelper cannot be null");

            return facade;
        }

        private void ReadProperties(string propertyName, ref Utf8JsonReader reader, CanvasFacade facade)
        {
            reader.Read();
            if (propertyName == nameof(CanvasFacade.SelectedDrawingStyle))
                facade.SelectedDrawingStyle = reader.GetInt32();
            else if (propertyName == nameof(CanvasFacade.GridDensity))
                facade.GridDensity = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.AutoComplete))
                facade.AutoComplete = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.Thickness))
                facade.Thickness = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.DeleteRange))
                facade.DeleteRange = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.SnapToGrid))
                facade.SnapToGrid = reader.GetBoolean();
            else if (propertyName == nameof(CanvasFacade.ShowGrid))
                facade.ShowGrid = reader.GetBoolean();
            else if (propertyName == nameof(CanvasFacade.DeleteMode))
                facade.DeleteMode = reader.GetBoolean();
            else
                throw new FormatException("Unexpected property");
        }

        public override void Write(Utf8JsonWriter writer, CanvasFacade value, JsonSerializerOptions options)
        {
            JsonSerializerOptions newOptions = new(options);
            newOptions.Converters.Add(new ProjectConverter());
            newOptions.Converters.Add(new IDrawingHelperConverter());
            newOptions.Converters.Add(new INamedValueConverter());
            writer.WriteStartObject();
            writer.WritePropertyName(nameof(CanvasFacade.Project));
            JsonSerializer.Serialize(writer, value.Project, newOptions);
            writer.WritePropertyName(nameof(CanvasFacade.DevicePrices));
            JsonSerializer.Serialize(writer, value.DevicePrices, newOptions);
            writer.WritePropertyName(nameof(CanvasFacade.DrawingTypes));
            JsonSerializer.Serialize(writer, value.DrawingTypes, newOptions);
            writer.WritePropertyName(nameof(CanvasFacade.DrawingHelper));
            JsonSerializer.Serialize(writer, value.DrawingHelper, newOptions);
            writer.WriteNumber(nameof(CanvasFacade.AutoComplete), value.AutoComplete);
            writer.WriteNumber(nameof(CanvasFacade.DeleteRange), value.DeleteRange);
            writer.WriteNumber(nameof(CanvasFacade.GridDensity), value.GridDensity);
            writer.WriteNumber(nameof(CanvasFacade.SelectedDrawingStyle), value.SelectedDrawingStyle);
            writer.WriteNumber(nameof(CanvasFacade.Thickness), value.Thickness);
            writer.WriteBoolean(nameof(CanvasFacade.ShowGrid), value.ShowGrid);
            writer.WriteBoolean(nameof(CanvasFacade.SnapToGrid), value.SnapToGrid);
            writer.WriteBoolean(nameof(CanvasFacade.DeleteMode), value.DeleteMode);

            writer.WriteEndObject();
        }
    }
}
