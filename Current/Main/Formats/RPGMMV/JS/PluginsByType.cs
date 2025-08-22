using Newtonsoft.Json.Linq;
using RPGMVJsonParser;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslationHelper.Projects;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class PluginsByType : JSBase
    {
        public override string JSName => "plugins.js";
        public override string JSSubfolder => string.Empty;

        public PluginsByType(ProjectBase parentProject) : base(parentProject)
        {
        }

        protected override bool JSTokenValid(JValue value)
        {
            return !value.Path.Contains("Modelname") && !value.Path.Contains("imageName");
        }

        protected override void FileOpen()
        {
            var data = File.ReadAllText(FilePath);

            var jsonString = Regex.Match(data, @"^([\s\S]*var\s+\$plugins\s*\=[^\[]*)([\s\S]+\]);\s*$");

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
                }
            }

            if (SaveFileMode && RET)
            {
                ParseData.ResultForWrite.Append(Helper.BuildJsonPluginsString(plugins));
            }
        }
    }
}
