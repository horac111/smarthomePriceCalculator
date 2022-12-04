using CanvasComponent.Abstract;
using System;

namespace CanvasComponent.Model
{
    public class NamedValue<T> : INamedValue<T>, IEquatable<T>, IEquatable<NamedValue<T>> where T : struct
    {
        public NamedValue() { }

        public NamedValue(string name, T value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public T Value { get; init; }

        public bool Equals(T other) => other.Equals(Value);

        public new bool Equals(object obj) => this.Equals(obj as NamedValue<T>);

        public bool Equals(NamedValue<T> other) => Value.Equals(other?.Value);

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }


    }
}
