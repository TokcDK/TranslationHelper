using TranslationHelper.Data;

namespace TranslationHelper.Functions.OnlineTranslation
{
    abstract class TranslationBase
    {
        protected THDataWork thDataWork;
        protected TranslationBase(THDataWork thDataWork)
        {
            this.thDataWork = thDataWork;
        }

        internal abstract void Get();
    }
}
