using System.ComponentModel;
using Config.Net;
using THConfig.Interfaces.SettingsGroups;

namespace THConfig.Interfaces
{
    //public interface IAppSettings : IGeneralSettings, IOptimizationSettings, IToolsSettings
    //{
    //}
    public interface IAppSettings
    {
        [DisplayName("Recursive DB Load")]
        [Description("Resursive scan of the loading translation DB. SLow but can find more translations")]
        [DefaultValue(false)]
        bool RecursiveDBLoad { get; set; }
        [DisplayName("Some integer property name")]
        [Description("Some description for integer")]
        [DefaultValue(90)]
        int IntProperty { get; set; }
        [DisplayName("Some string property name")]
        [Description("Some description for string")]
        [DefaultValue("testvalue dddddddddddddddddddddddddddddddddddddddddddd")]
        string StringProperty { get; set; }
    }
}
