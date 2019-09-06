using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace TranslationHelper
{
    static public class THCreateSymlink
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool CreateSymbolicLink(
        string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags);

        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public static void Folder(string folderforwichcreate, string symlinkwherecreate)
        {
            CreateSymbolicLink(symlinkwherecreate, folderforwichcreate, SymbolicLink.Directory);
        }

        public static void File(string fileforwichcreate, string symlinkwherecreate)
        {            
            CreateSymbolicLink(symlinkwherecreate, fileforwichcreate, SymbolicLink.File);
        }
    }
}
