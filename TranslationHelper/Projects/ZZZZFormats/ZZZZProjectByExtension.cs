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

        private void GetValidOpenable()
        {
            formatsTypes = GetListOfSubClasses.Inherited.GetListOfInheritedTypes(typeof(FormatStringBase));
        }

        List<Type> formatsTypes;
        internal override bool Check()
        {
            GetValidOpenable();

            var fileExt = Path.GetExtension(ProjectData.SelectedFilePath);
            List<Type> foundTypes = new List<Type>();
            foreach (var formatType in formatsTypes)
            {
                var format = (FormatBase)Activator.CreateInstance(formatType);
                if (format.Ext() == fileExt && format.ExtIdentifier() > -1)
                {
                    foundTypes.Add(formatType);
                }
            }

            int selectedIndex = -1;
            if (foundTypes.Count > 0)
            {
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
            }

            if (selectedIndex > -1)
            {
                CurrentFormat = (FormatBase)Activator.CreateInstance(foundTypes[selectedIndex]);
                return true;
            }

            return false;
        }

        internal override string Name()
        {
            return string.IsNullOrWhiteSpace(CurrentFormat.Name()) ? CurrentFormat.Ext() : CurrentFormat.Name();
        }

        internal override bool Open() => OpenSave();

        internal override bool Save() => OpenSave();

        bool OpenSave()
        {
            var dir = Path.GetDirectoryName(ProjectData.SelectedFilePath);
            var ext = CurrentFormat.Ext();
            int extCnt = 0;
            foreach (var i in Directory.EnumerateFiles(dir, "*" + ext)) if (++extCnt > 1) break;

            bool getAll = false;
            if (extCnt > 1 && (ProjectData.SaveFileMode || MessageBox.Show(T._("Found similar files. Open them too?"), T._("Found files with same extension"), MessageBoxButtons.YesNo) == DialogResult.Yes))
            {
                getAll = true;
            }

            return OpenSaveFilesBase(new DirectoryInfo(dir), CurrentFormat.GetType(), getAll ? "*" + ext : Path.GetFileName(CurrentFormat.FilePath), false, searchOption: SearchOption.TopDirectoryOnly);
        }
    }
}
