using Alphaleonis.Win32.Filesystem;

namespace TranslationHelper.ExternalAdditions
{
    static class HardLinks
    {
        /// <summary>
        /// Create hardlink in path <paramref name="hardlinkPath"/> for file <paramref name="existingFilePath"/>
        /// </summary>
        /// <param name="existingFilePath"></param>
        /// <param name="hardlinkPath"></param>
        public static void CreateHardlink(this string existingFilePath, string hardlinkPath)
        {
            File.CreateHardLink(hardlinkPath, existingFilePath);
        }
    }
}
