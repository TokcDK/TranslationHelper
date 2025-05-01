using System;

namespace TranslationHelper.Functions.FileElementsFunctions.Row
{
    public class TranslationCache : ITranslationCache
    {
        bool _disposed = false;

        public TranslationCache()
        {
            FunctionsOnlineCache.Init(this);
            Read();
        }

        public string TryGetValue(string key)
        {
            return FunctionsOnlineCache.TryGetValue(key);
        }

        public void TryAdd(string key, string value)
        {
            FunctionsOnlineCache.TryAdd(key, value);
        }

        public void Write()
        {
            FunctionsOnlineCache.Write();
        }

        static void Read()
        {
            FunctionsOnlineCache.Read();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Write();

                FunctionsOnlineCache.Unload(this);

                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }
}