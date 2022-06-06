using RPGMVJsonParser;
using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Formats.RPGMMV.JsonParser;

namespace TranslationHelper.Formats.RPGMMV
{
    abstract class RPGMMVBase : FormatStringBase
    {
        protected bool ParseList(ref string s, string info)
        {
            bool RET = false;
            if (s.StartsWith("[") && s.EndsWith("]")) // is json array
            {
                bool ret = false;
                var parameterData = Helper.LoadArray(s);
                for (int i1 = 0; i1 < parameterData.Count; i1++)
                {
                    if (parameterData[i1] is string s1) { } else continue;

                    if (ParseList(ref s1, info) && ProjectData.SaveFileMode)
                    {
                        RET = ret = true;
                        parameterData[i1] = s1;
                    }
                }

                if (ProjectData.SaveFileMode && ret) s = Helper.Json2String(parameterData);
            }
            else if (s.StartsWith("{") && s.EndsWith("}")) // is json array
            {
                bool ret = false;
                var parameterData = Helper.LoadDictionary(s);
                var parameterKeys = new List<string>(parameterData.Keys);
                foreach (var parameterKey in parameterKeys)
                {
                    if (parameterData[parameterKey] is string s1) { } else continue;

                    if (ParseList(ref s1, info + $"\r\nSubkey: {parameterKey}") && ProjectData.SaveFileMode)
                    {
                        RET = ret = true;
                        parameterData[parameterKey] = s1;
                    }
                }

                if (ProjectData.SaveFileMode && ret) s = Helper.Json2String(parameterData);
            }
            else
            {
                if (AddRowData(ref s, info) && ProjectData.SaveFileMode)
                {
                    RET = true;
                    //plugin.Parameters[key] = s;
                }
            }

            return RET;
        }
    }
}
