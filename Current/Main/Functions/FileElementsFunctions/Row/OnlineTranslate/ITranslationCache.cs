using System;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    /// <summary>
    /// Defines the contract for translation caching.
    /// </summary>
    public interface ITranslationCache : IDisposable
    {
        string TryGetValue(string key);
        void TryAdd(string key, string value);

        /// <summary>
        /// For purpose if need to write cache earlier
        /// </summary>
        void Write();
    }
}