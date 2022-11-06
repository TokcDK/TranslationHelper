using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;
using THConfig.Interfaces;

namespace THConfig
{
    public class Manage
    {
        public static void Load(string startupPath, string productName)
        {
            Load(startupPath, productName, StaticSettings.ApplicationIniPath);
        }
        public static void Load(string startupPath, string productName, string iniFilePath)
        {
            // set app exe name, startup path and ini file path
            StaticSettings.ApplicationStartupPath = startupPath;
            StaticSettings.ApplicationProductName = productName;
            if (!string.IsNullOrEmpty(iniFilePath)) StaticSettings.AppIniPath = iniFilePath;

            // set main settings
            StaticSettings.Settings = new ConfigurationBuilder<IAppSettings>()
           .UseIniFile(iniFilePath)
           .Build();
        }
    }
}
