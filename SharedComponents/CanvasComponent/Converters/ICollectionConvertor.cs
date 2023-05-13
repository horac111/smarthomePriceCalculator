using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CanvasComponent.Converters
{
    public class ICollectionConvertor<T, R> : JsonConverter<ICollection<T>> where R : JsonConverter<T>
    {
        public override ICollection<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var convertor = Activator.CreateInstance<R>();
            JsonSerializerOptions newOptions = new();
            newOptions.Converters.Add(convertor);
            return JsonSerializer.Deserialize<ICollection<T>>(ref reader, newOptions);
        }

        public override void Write(Utf8JsonWriter writer, ICollection<T> value, JsonSerializerOptions options)
        {
            var convertor = Activator.CreateInstance<R>();
            JsonSerializerOptions newOptions = new();
            newOptions.Converters.Add(convertor);
            JsonSerializer.Serialize(writer, value, newOptions);
        }
    }
}
