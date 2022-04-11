using KenshModTIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TranslationHelper.Data;
using TranslationHelper.Formats.LOFI.Kenshi;
using TranslationHelper.Projects.ZZZZFormats;

namespace TranslationHelper.Projects.LO_FI
{
    internal class KenshiMod : ProjectBase
    {
        internal override bool Check()
        {
            bool b1 = IsExe();
            bool b2 = Path.GetFileName(ProjectData.SelectedFilePath).StartsWith("kenshi");
            bool b3 = Directory.Exists(Path.Combine(Path.GetDirectoryName(ProjectData.SelectedFilePath), "mods"));
            return b1 && b2 && b3;
        }

        internal override string Name()
        {
            return "Kenshi mod";
        }

        internal override bool Open()
        {
            var gamePath = Path.GetDirectoryName(ProjectData.SelectedFilePath);
            List<string> foundTypes = new List<string>();
            foreach(var dir in Directory.GetDirectories(Path.Combine(gamePath, "mods")))
            {
                var modFilePath = Path.Combine(dir, Path.GetFileNameWithoutExtension(dir)+".mod");

                if (File.Exists(modFilePath)) foundTypes.Add(Path.GetFileNameWithoutExtension(modFilePath));
            }

            int selectedIndex = -1;
            if (foundTypes.Count == 0) return false;

            var foundForm = new FoundTypesbyExtensionForm();

            foreach (var type in foundTypes)
            {
                foundForm.listBox1.Items.Add(type);
            }

            var result = foundForm.ShowDialog();
            if (result == DialogResult.OK)
            {
                selectedIndex = foundForm.SelectedTypeIndex;
            }

            foundForm.Dispose();

            if (selectedIndex == -1) return false;

            var modName = foundTypes[selectedIndex];

            var format = new MOD
            {
                FilePath = modName
            };

            return format.Open();
        }

        internal override bool Save()
        {
            throw new NotImplementedException();
        }
    }
}
