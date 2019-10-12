using System;
using System.Collections.Generic;
using System.Text;

namespace BridgeTimer
{
    public static class CollectionExtensions
    {
        public static TValue SafeGet<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,
                                                  TKey key,
                                                  TValue defaultValue=default(TValue))
            where TKey:notnull  
        {
            if (!dictionary.ContainsKey(key))
                return defaultValue;
            else
                return dictionary[key];
        }

        public static void AddOrEdit<TKey,TValue>(this IDictionary<TKey,TValue> dictionary,TKey key, TValue value)
            where TKey:notnull
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);
        }
    }
}
