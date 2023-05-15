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
            newOptions.Converters.Add(new DevicePriceItemConverter());

            CanvasFacade facade = new();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    break;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new FormatException("Expected property name");

                var propertyName = reader.GetString().ToLower();
                if (propertyName == nameof(CanvasFacade.Project).ToLower())
                    facade.Project = JsonSerializer.Deserialize<Project>(ref reader, newOptions);
                else if (propertyName == nameof(CanvasFacade.DevicePrices).ToLower())
                    JsonSerializer.Deserialize<IEnumerable<DevicePriceItem>>(ref reader, newOptions);
                else if (propertyName == nameof(CanvasFacade.DrawingTypes).ToLower())
                    JsonSerializer.Deserialize<IEnumerable<INamedValue<int>>>(ref reader, newOptions);
                else if (propertyName == nameof(CanvasFacade.DrawingHelper).ToLower())
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
            if (propertyName == nameof(CanvasFacade.SelectedDrawingStyle).ToLower())
                facade.SelectedDrawingStyle = reader.GetInt32();
            else if (propertyName == nameof(CanvasFacade.GridDensity).ToLower())
                facade.GridDensity = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.AutoComplete).ToLower())
                facade.AutoComplete = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.Thickness).ToLower())
                facade.Thickness = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.DeleteRange).ToLower())
                facade.DeleteRange = reader.GetDouble();
            else if (propertyName == nameof(CanvasFacade.SnapToGrid).ToLower())
                facade.SnapToGrid = reader.GetBoolean();
            else if (propertyName == nameof(CanvasFacade.ShowGrid).ToLower())
                facade.ShowGrid = reader.GetBoolean();
            else if (propertyName == nameof(CanvasFacade.DeleteMode).ToLower())
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
            newOptions.Converters.Add(new DevicePriceItemConverter());
            newOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

            writer.WriteStartObject();
            writer.WritePropertyName(nameof(CanvasFacade.Project).ToLower());
            JsonSerializer.Serialize(writer, value.Project, newOptions);
            writer.WritePropertyName("devicePrices");
            JsonSerializer.Serialize(writer, value.DevicePrices, newOptions);
            writer.WritePropertyName("drawingTypes");
            JsonSerializer.Serialize(writer, value.DrawingTypes, newOptions);
            writer.WritePropertyName("drawingHelper");
            JsonSerializer.Serialize(writer, value.DrawingHelper, newOptions);
            writer.WriteNumber("autoComplete", value.AutoComplete);
            writer.WriteNumber("deleteRange", value.DeleteRange);
            writer.WriteNumber("gridDensity", value.GridDensity);
            writer.WriteNumber("selectedDrawingStyle", value.SelectedDrawingStyle);
            writer.WriteNumber(nameof(CanvasFacade.Thickness).ToLower(), value.Thickness);
            writer.WriteBoolean("showGrid", value.ShowGrid);
            writer.WriteBoolean("snapToGrid", value.SnapToGrid);
            writer.WriteBoolean("deleteMode", value.DeleteMode);

            writer.WriteEndObject();
        }
    }
}
