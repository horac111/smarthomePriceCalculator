using CanvasComponent.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace CanvasComponent.Model
{
    public class PartialReadOnlyDictionary<T, R, TInner> : IDictionary<T, R> where T : INamedValue<TInner>
    {
        private Dictionary<T, R> dictionary = new();

        private Dictionary<TInner, T> keys = new();

        private Dictionary<T, int> indexes = new();

        private int unchangebleRecords;

        public ICollection<T> Keys => dictionary.Keys;

        public ICollection<R> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public R this[T key]
        {
            get
            {
                if (indexes[key] < unchangebleRecords)
                    return clone(dictionary[key]);

                return dictionary[key];
            }
            set
            {
                if (indexes[key] < unchangebleRecords)
                    throw new ArgumentException($"Property with {JsonSerializer.Serialize(key)} is unchangeble.");
                dictionary[key] = value;
            }
        }

        public R this[TInner key]
        {
            get => this[keys[key]];
            set => this[keys[key]] = value;
        }

        internal R GetChangable(T key)
            => dictionary[key];

        internal R GetChangable(TInner key)
        {
            return dictionary[keys[key]];
        }


        public PartialReadOnlyDictionary(int numberOfRecords)
        {
            unchangebleRecords = numberOfRecords;
        }

        public void Add(T key, R value)
        {
            indexes.Add(key, Count);
            dictionary.Add(key, value);
            keys.Add(key.Value, key);
            GC.KeepAlive(key);
            GC.KeepAlive(value);
            GC.KeepAlive(dictionary);
            GC.KeepAlive(keys);
        }

        public bool ContainsKey(T key) => dictionary.ContainsKey(key);
        public bool ContainsKey(TInner key) => keys.ContainsKey(key);

        public bool Remove(T key)
        {
            if (ContainsKey(key))
            {
                if (indexes[key] < unchangebleRecords)
                    throw new ArgumentException($"Property with {JsonSerializer.Serialize(key)} is unchangeble.");
                dictionary.Remove(key);
                var index = indexes[key];
                indexes.Remove(key);
                keys.Remove(key.Value);
                foreach (var pair in indexes)
                    if (pair.Value > index)
                        indexes[pair.Key]--;
                return true;
            }
            return false;
        }

        public bool TryGetValue(T key, [MaybeNullWhen(false)] out R value)
        {
            if (ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default;
            return false;
        }

        public bool TryGetValue(TInner key, [MaybeNullWhen(false)] out R value)
        => TryGetValue(keys[key], out value);

        public void Add(KeyValuePair<T, R> item) => Add(item.Key, item.Value);

        public void Clear()
        {
            indexes = new();
            dictionary = new();
        }

        public bool Contains(KeyValuePair<T, R> item)
            => ContainsKey(item.Key) && dictionary[item.Key].Equals(item.Value);

        public void CopyTo(KeyValuePair<T, R>[] array, int arrayIndex)
        => dictionary.ToArray().CopyTo(array, arrayIndex);

        public bool Remove(KeyValuePair<T, R> item)
        => Remove(item.Key);

        public IEnumerator<KeyValuePair<T, R>> GetEnumerator()
        => dictionary.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();

        private R clone(R obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                if (typeof(R).IsSerializable)
                {
                    JsonSerializer.Serialize(stream, obj);
                    stream.Position = 0;
                    return JsonSerializer.Deserialize<R>(stream);
                }
                return default;
            }
        }
    }
}
