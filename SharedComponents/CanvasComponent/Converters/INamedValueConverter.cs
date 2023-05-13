﻿using CanvasComponent.Abstract;
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
    public class INamedValueConverter : JsonConverter<INamedValue<int>>
    {
        public override INamedValue<int> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            int value = 0;
            string name = "";
            for (int i = 0; i < 2; i++)
            {
                reader.Read();
                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new FormatException("Expected property name");
                var propertyName = reader.GetString();
                reader.Read();
                switch (propertyName)
                {
                    case nameof(INamedValue<int>.Name):
                        name = reader.GetString();
                        break;
                    case nameof(INamedValue<int>.Value):
                        value = reader.GetInt32();
                        break;
                    default:
                        throw new FormatException("Unknown format");
                }
            }
            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
                throw new FormatException("Expected end object");
            return new NamedValue<int>(name, value);
        }

        public override void Write(Utf8JsonWriter writer, INamedValue<int> value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString(nameof(INamedValue<int>.Name), value.Name);
            writer.WriteNumber(nameof(INamedValue<int>.Value), value.Value);
            writer.WriteEndObject();
        }
    }
}