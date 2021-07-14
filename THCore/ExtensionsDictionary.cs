
using System.Collections.Generic;

public static class ExtensionsDictionary
{
    /// <summary>
    /// Add Key-Value if Key not exists
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="Dict"></param>
    /// <param name="Key"></param>
    /// <param name="Value"></param>
    internal static void AddTry<T>(this Dictionary<T, T> Dict, T Key, T Value)
    {
        if (Key != null && !Dict.ContainsKey(Key) && (Value != null || (Value is string v && !string.IsNullOrEmpty(v))) && !Equals(Key, Value))
        {
            Dict.Add(Key, Value);
        }
    }
}