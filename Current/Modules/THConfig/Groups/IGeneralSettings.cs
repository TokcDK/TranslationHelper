using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace THConfig.Groups
{
    public interface IGeneralSettings
    {
        bool RecursiveDBLoad { get; }
        int LineSplitCharsLimit { get; }
        string SourceLanguage { get; }
    }
}
