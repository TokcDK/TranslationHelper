using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;
using THConfig.Interfaces;

namespace THConfig
{
    public class ConfigManage
    {
        public static IAppSettings Settings = null;
        public static void Load()
        {
            Settings = new ConfigurationBuilder<IAppSettings>()
           .UseAppConfig()
           .UseIniFile(StaticSettings.ApplicationIniPath)
           .Build();
        }
    }
}
