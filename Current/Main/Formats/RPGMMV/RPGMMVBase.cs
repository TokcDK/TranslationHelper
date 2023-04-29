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
                try
                {
                    List<object> parameterData = Helper.LoadArray(s);

                    if (parameterData==null || parameterData.Count==0) return ParseString(ref s, info);

                    for (int i1 = 0; i1 < parameterData.Count; i1++)
                    {
                        if (parameterData[i1] is string s1) { } else continue;

                        if (ParseList(ref s1, info) && SaveFileMode)
                        {
                            RET = ret = true;
                            parameterData[i1] = s1;
                        }
                    }

                    if (SaveFileMode && ret) s = Helper.Json2String(parameterData);
                }
                catch
                {
                    return ParseString(ref s, info); // parse as string, when parse as json elements was not possible
                }
            }
            else if (s.StartsWith("{") && s.EndsWith("}")) // is json array
            {
                bool ret = false;
                try
                {
                    Dictionary<string, object> parameterData = Helper.LoadDictionary(s);
                    if (parameterData == null || parameterData.Count == 0) return ParseString(ref s, info);

                    var parameterKeys = new List<string>(parameterData.Keys);
                    foreach (var parameterKey in parameterKeys)
                    {
                        if (parameterData[parameterKey] is string s1) { } else continue;

                        if (ParseList(ref s1, info + $"\r\nSubkey: {parameterKey}") && SaveFileMode)
                        {
                            RET = ret = true;
                            parameterData[parameterKey] = s1;
                        }
                    }

                    if (SaveFileMode && ret) s = Helper.Json2String(parameterData);
                }
                catch
                {
                    return ParseString(ref s, info); // parse as string, when parse as json elements was not possible
                }
            }
            else
            {
                return ParseString(ref s , info);
            }

            return RET;
        }

        public bool ParseString(ref string s, string info)
        {
            return AddRowData(ref s, info);
        }
    }
}
