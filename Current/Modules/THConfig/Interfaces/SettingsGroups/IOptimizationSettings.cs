using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THConfig.Interfaces.SettingsGroups
{
    public interface IOptimizationSettings
    {
        [DefaultValue(true)]
        bool AutoTranslateForSimilar { get; set; }
        [DefaultValue(90)]
        int DontLoadStringIfRomajiPercent { get; set; }
        [DefaultValue("Gzip (cmx)")]
        string DBCompressionFormat { get; set; }
        [DefaultValue(true)]
        bool IgnoreOriginalEqualsTranslationLines { get; set; }
        [DefaultValue(false)]
        bool DontLoadDuplicates { get; set; }
    }
}
