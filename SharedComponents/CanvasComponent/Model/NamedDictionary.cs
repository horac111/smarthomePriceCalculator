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
    public class NamedDictionary<T, R, TInner> : IDictionary<T, R> where T : INamedValue<TInner>
    {
        private Dictionary<T, R> dictionary = new();

        private Dictionary<TInner, T> keys = new();


        public ICollection<T> Keys => dictionary.Keys;

        public ICollection<R> Values => dictionary.Values;

        public int Count => dictionary.Count;

        public bool IsReadOnly => false;

        public R this[T key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public R this[TInner key]
        {
            get => this[keys[key]];
            set => this[keys[key]] = value;
        }


        public NamedDictionary()
        {
        }

        public void Add(T key, R value)
        {
            dictionary.Add(key, value);
            keys.Add(key.Value, key);
        }

        public bool ContainsKey(T key) 
            => key is not null && (dictionary.ContainsKey(key) || keys.ContainsKey(key.Value));
        public bool ContainsKey(TInner key) => key is not null && keys.ContainsKey(key);

        public bool Remove(T key)
        {
            if (key is null)
                return false;

            if (ContainsKey(key))
            {
                key = keys[key.Value];
                dictionary.Remove(key);
                keys.Remove(key.Value);
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
            dictionary = new();
            keys = new();
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
    }
}
