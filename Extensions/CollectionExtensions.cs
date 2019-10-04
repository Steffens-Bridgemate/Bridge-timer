using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeTimer
{
    public static class CollectionExtensions
    {
        public static TValue SafeGet<TKey,TValue>(this IDictionary<TKey,TValue> dictionary, TKey key)
        {
            if (!dictionary.ContainsKey(key))
                return default(TValue);
            else
                return dictionary[key];
        }

        public static void AddOrEdit<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
