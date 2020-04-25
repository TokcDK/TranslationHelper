using System.Collections.Generic;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class JSSharedData
    {
        internal readonly List<JSBase> ListOfJS;

        public JSSharedData(THDataWork thDataWork)
        {
            ListOfJS = new List<JSBase>
            {
                new PLUGINS(thDataWork)
                ,
                new RECOLLECTIONMODE(thDataWork)
            };
        }
    }
}
