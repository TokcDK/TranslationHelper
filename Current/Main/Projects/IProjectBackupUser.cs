using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper.Projects
{
    public interface IProjectBackupUser
    {
        List<string> BakPaths { get; set; }

        bool BakCreate();

        bool BakRestore();
    }
}
