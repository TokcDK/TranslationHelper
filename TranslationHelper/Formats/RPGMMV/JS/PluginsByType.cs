using Newtonsoft.Json.Linq;
using RPGMVJsonParser;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Data;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PluginsByType : JSBase
    {
        public PluginsByType()
        {
        }

        internal override int ExtIdentifier => (Path.GetFileName(AppData.SelectedFilePath).ToUpperInvariant() == "PLUGINS.JS" && Path.GetFileName(Path.GetDirectoryName(AppData.SelectedFilePath)).ToUpperInvariant() == "JS" ? 1 : -1);

        protected override bool JSTokenValid(JValue value)
        {
            return !value.Path.Contains("Modelname") && !value.Path.Contains("imageName");
        }

        protected override void FileOpen()
        {
            var data = File.ReadAllText(FilePath);

            var jsonString = Regex.Match(data, @"(var\s+\$plugins\s*\=)([\s\S]+);$");

            if (!jsonString.Success) return;

            var plugins = Helper.LoadPlugins(jsonString.Groups[2].Value, true);

            int pluginsCount = plugins.Count;
            for (int i = 0; i < pluginsCount; i++)
            {
                var plugin = plugins[i];
                var keys = new List<string>(plugin.Parameters.Keys);
                foreach (var key in keys)
                {
                    if (plugin.Parameters[key] is string s) { } else continue;

                    if (ParseList(ref s, $"Plugin name:{plugin.Name}\r\nKey: {key}\r\nNote: Be careful with translating this!") && SaveFileMode)
                    {
                        plugin.Parameters[key] = s;
                    }

                    //if (s.StartsWith("[") && s.EndsWith("]")) // is json array
                    //{
                    //    bool ret = false;
                    //    var parameterData = Helper.LoadArray(s);
                    //    for (int i1 = 0; i1 < parameterData.Count; i1++)
                    //    {
                    //        if (parameterData[i1] is string s1) { } else continue;

                    //        if (AddRowData(ref s1, $"Plugin name:{plugin.Name}\r\nKey: {key}\r\nNote: Be careful with translating this!") && ProjectData.SaveFileMode)
                    //        {
                    //            ret = true;
                    //            parameterData[i1] = s1;
                    //        }
                    //    }

                    //    if (ProjectData.SaveFileMode && ret) plugin.Parameters[key] = Helper.Json2String(parameterData);
                    //}
                    //else if (s.StartsWith("{") && s.EndsWith("}")) // is json array
                    //{
                    //    bool ret = false;
                    //    var parameterData = Helper.LoadDictionary(s);
                    //    var parameterKeys = new List<string>(parameterData.Keys);
                    //    foreach (var parameterKey in parameterKeys)
                    //    {
                    //        if (parameterData[parameterKey] is string s1) { } else continue;

                    //        if (AddRowData(ref s1, $"Plugin name:{plugin.Name}\r\nKey: {key}\r\nNote: Be careful with translating this!") && ProjectData.SaveFileMode)
                    //        {
                    //            ret = true;
                    //            parameterData[parameterKey] = s1;
                    //        }
                    //    }

                    //    if (ProjectData.SaveFileMode && ret) plugin.Parameters[key] = Helper.Json2String(parameterData);
                    //}
                    //else
                    //{
                    //    if (AddRowData(ref s, $"Plugin name:{plugin.Name}\r\nKey: {key}\r\nNote: Be careful with translating this!") && ProjectData.SaveFileMode)
                    //    {
                    //        plugin.Parameters[key] = s;
                    //    }
                    //}

                }
            }

            if (SaveFileMode && RET) ParseData.ResultForWrite.Append("var $plugins =" + Helper.Json2String(plugins) + ";$");
        }

        //protected override KeywordActionAfter ParseStringFileLine()
        //{
        //    if (ParseData.TrimmedLine.TrimStart().StartsWith("{\"name\":"))
        //    {
        //        try
        //        {
        //            if (ParseData.Line.EndsWith(","))
        //            {
        //                IsJsonNotLast = true;
        //                ParseData.Line = ParseData.Line.Remove(ParseData.Line.Length - 1, 1);
        //            }
        //            else if (ProjectData.SaveFileMode)
        //            {
        //                IsJsonNotLast = false;
        //            }
        //        }
        //        catch
        //        {
        //        }

        //        IsJsonNotLast = !IsJsonNotLast;
        //    }

        //    SaveModeAddLine(newline: "\n");

        //    return KeywordActionAfter.Continue;
        //}
        private bool IsJsonNotLast;

        public override string JSName => "plugins.js";
        public override string JSSubfolder => string.Empty;
    }
}
