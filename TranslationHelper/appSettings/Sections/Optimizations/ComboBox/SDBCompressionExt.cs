using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;

namespace TranslationHelper.INISettings
{
    class SDBCompressionExt : Optimizations
    {
        public SDBCompressionExt()
        {
            if (ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Count == 0)
            {
                formats = FunctionsInterfaces.GetDBSaveFormats();
                foreach (var ext in formats)
                {
                    ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add(ext.Description);
                }

                //ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("XML (none)");
                //ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Gzip (cmx)");
                //ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Deflate (cmz)");
            }
        }

        List<IDBSave> formats;
        readonly IDBSave defaultformat = new XML();

        internal override string Key => "DBCompressionExt";

        internal override string Default => defaultformat.Description;

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                Properties.Settings.Default.DBCompressionExt = ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.SelectedItem.ToString();
            }
            else
            {
                Properties.Settings.Default.DBCompressionExt = IsExistsInFormats(ProjectData.BufferValueString) ? ProjectData.BufferValueString : Default;
                ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.SelectedItem = Properties.Settings.Default.DBCompressionExt;
            }
        }

        private bool IsExistsInFormats(string bufferValueString)
        {
            if(formats == null)
            {
                formats = FunctionsInterfaces.GetDBSaveFormats();
            }
            foreach (var format in formats)
            {
                if (bufferValueString == format.Description)
                {
                    return true;
                }
            }
            return false;
        }

        internal override string Get()
        {
            return Properties.Settings.Default.DBCompressionExt;
        }

        internal override string ID()
        {
            return ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Name;
        }
    }
}
