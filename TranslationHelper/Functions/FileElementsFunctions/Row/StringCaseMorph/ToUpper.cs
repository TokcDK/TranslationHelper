using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUpper : StringCaseMorphBase
    {
        public ToUpper(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override int variant => 1;
    }
}
