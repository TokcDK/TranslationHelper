using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    class ToUPPER : StringCaseMorphBase
    {
        public ToUPPER(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override int variant => 2;
    }
}
