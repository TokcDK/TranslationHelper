using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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
        [DefaultValue(false)]
        bool RecursiveDBLoad { get; set; }
        [DefaultValue(60)]
        int LineSplitCharsLimit { get; set; }
        [DefaultValue("Japanese jp")]
        string SourceLanguage { get; set; }
        [DefaultValue(true)]
        bool EnableAutoSave { get; set; }
        [DefaultValue(90)]
        int AutoSaveCooldownSec { get; set; }
    }
}
