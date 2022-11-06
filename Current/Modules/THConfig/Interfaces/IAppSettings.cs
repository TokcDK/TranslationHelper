using Config.Net;
using THConfig.Interfaces.SettingsGroups;

namespace THConfig.Interfaces
{
    public interface IAppSettings : IGeneralSettings, IOptimizationSettings, IToolsSettings
    {
    }
}
