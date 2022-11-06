using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THConfig.Groups
{
    public interface IOptimizationSettings
    {
        [DefaultValue("True")]
        bool AutoTranslateForSimilar { get; set; }
        [DefaultValue("90")]
        bool DontLoadStringIfRomajiPercent { get; set; }
        [DefaultValue("Gzip (cmx)")]
        string DBCompressionFormat { get; set; }
        [DefaultValue("True")]
        bool IgnoreOriginalEqualsTranslationLines { get; set; }
        [DefaultValue("False")]
        bool DontLoadDuplicates { get; set; }
    }
}
