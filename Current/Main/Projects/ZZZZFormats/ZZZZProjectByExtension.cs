using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats;

namespace TranslationHelper.Projects.ZZZZFormats
{
    class ZZZZProjectByExtension : ProjectBase
    {
        /// <summary>
        /// Project will be set be specified extension of found format
        /// </summary>
        public ZZZZProjectByExtension()
        {
        }

        public override bool IsSaveToSourceFile => true; // we opened standalone file and will write in it

        internal override bool IsValid()
        {
            var fileExt = Path.GetExtension(AppData.SelectedFilePath);            
            foreach (var formatType in GetFormatTypes(typeof(FormatBase))) 
                if (IsValidFormat(formatType, fileExt)) return true;

            return false;
        }

        private static IEnumerable<Type> GetFormatTypes(Type type)
        {
            foreach(var fType in 
                GetListOfSubClasses.Inherited.GetInheritedTypes(type)) yield return fType;
        }

        private static bool IsValidFormat(Type formatType, string fileExt)
        {
            var format = (IFormat)Activator.CreateInstance(formatType);
            
            return string.Equals(format.Extension, fileExt, StringComparison.InvariantCultureIgnoreCase);
        }

        FormatBase Format;

        public override string Name => string.IsNullOrWhiteSpace(Format.Name) ? Format.Extension: Format.Name;

        public override bool Open()
        {
            var fileExt = Path.GetExtension(AppData.SelectedFilePath);
            var foundTypes = new List<Type>();
            foreach (var formatType in GetFormatTypes(typeof(FormatBase))) 
                if (IsValidFormat(formatType, fileExt)) foundTypes.Add(formatType);

            if (foundTypes.Count == 0) return false;

            int selectedIndex = -1;
            var foundForm = new FoundTypesbyExtensionForm();
            foreach (var type in foundTypes) foundForm.listBox1.Items.Add(type.FullName);

            var result = foundForm.ShowDialog();
            if (result == DialogResult.OK) selectedIndex = foundForm.SelectedTypeIndex;

            foundForm.Dispose();

            if (selectedIndex == -1) return false;

            Format = (FormatBase)Activator.CreateInstance(foundTypes[selectedIndex]);

            return OpenSave();
        }

        public override bool Save() => OpenSave();

        bool OpenSave()
        {
            var dir = Path.GetDirectoryName(AppData.SelectedFilePath);
            var ext = Format.Extension;
            int extCnt = 0;
            foreach (var i in Directory.EnumerateFiles(dir, "*" + ext)) if (++extCnt > 1) break;

            bool getAll = false;
            if (extCnt > 1 && (SaveFileMode || MessageBox.Show(T._("Found similar files. Open them too?"), T._("Found files with same extension"), MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                getAll = true;
            }

            return OpenSaveFilesBase(new DirectoryInfo(dir), Format.GetType(), getAll ? "*" + ext : Path.GetFileName(Format.FilePath), false, searchOption: SearchOption.TopDirectoryOnly);
        }
    }
}
