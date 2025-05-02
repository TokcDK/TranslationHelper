using System;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate
{
    public class TranslationCache : TranslationCacheBase
    {
        bool _disposed = false;

        public TranslationCache()
        {
            Init(this);
        }

        public override void Dispose()
        {
            if (!_disposed)
            {
                Write();

                Unload(this);

                _disposed = true;

                GC.SuppressFinalize(this);
            }
        }
    }
}