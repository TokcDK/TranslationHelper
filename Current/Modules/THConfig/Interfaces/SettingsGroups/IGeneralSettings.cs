using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Config.Net;

namespace THConfig.Interfaces.SettingsGroups
{
    public interface IGeneralSettings
    {
        [DisplayName("Recursive DB Load")]
        [Description("Resursive scan of the loading translation DB. SLow but can find more translations")]
        [DefaultValue("False")]
        bool RecursiveDBLoad { get; set; }
        [DefaultValue("60")]
        int LineSplitCharsLimit { get; set; }
        [DefaultValue("Japanese jp")]
        string SourceLanguage { get; set; }
        [DefaultValue("True")]
        bool EnableAutoSave { get; set; }
        [DefaultValue("90")]
        int AutoSaveCooldownSec { get; set; }
    }
}
