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

        internal override bool Check()
        {
            var fileExt = Path.GetExtension(AppData.SelectedFilePath);
            foreach (var formatType in GetListOfSubClasses.Inherited.GetInheritedTypes(typeof(FormatStringBase)))
            {
                var format = (FormatBase)Activator.CreateInstance(formatType);
                if (format.Ext== fileExt && format.ExtIdentifier> -1)
                {
                    return true;
                }
            }

            return false;
        }

        FormatBase Format;

        internal override string Name => string.IsNullOrWhiteSpace(Format.Name) ? Format.Ext: Format.Name;

        public override bool Open()
        {
            var fileExt = Path.GetExtension(AppData.SelectedFilePath);
            List<Type> foundTypes = new List<Type>();
            foreach (var formatType in GetListOfSubClasses.Inherited.GetInheritedTypes(typeof(FormatStringBase)))
            {
                var format = (FormatBase)Activator.CreateInstance(formatType);
                if (format.Ext== fileExt && format.ExtIdentifier> -1)
                {
                    foundTypes.Add(formatType);
                }
            }

            if (foundTypes.Count == 0) return false;

            int selectedIndex = -1;
            var foundForm = new FoundTypesbyExtensionForm();
            foreach (var type in foundTypes)
            {
                foundForm.listBox1.Items.Add(type.FullName);
            }

            var result = foundForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedIndex = foundForm.SelectedTypeIndex;
            }

            foundForm.Dispose();

            if (selectedIndex == -1) return false;

            Format = (FormatBase)Activator.CreateInstance(foundTypes[selectedIndex]);

            return OpenSave();
        }

        public override bool Save() => OpenSave();

        bool OpenSave()
        {
            var dir = Path.GetDirectoryName(AppData.SelectedFilePath);
            var ext = Format.Ext;
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
