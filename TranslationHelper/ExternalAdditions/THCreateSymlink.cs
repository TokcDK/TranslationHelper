using TranslationHelper.ExternalAdditions;

namespace TranslationHelper
{
    static public class ThCreateSymlink
    {
        internal enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        public static void Folder(string folderforwichcreate, string symlinkwherecreate)
        {
            NativeMethods.CreateSymbolicLink(symlinkwherecreate, folderforwichcreate, SymbolicLink.Directory);
        }

        public static void File(string fileforwichcreate, string symlinkwherecreate)
        {
            NativeMethods.CreateSymbolicLink(symlinkwherecreate, fileforwichcreate, SymbolicLink.File);
        }
    }
}
