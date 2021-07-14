using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;

namespace TranslationHelper.INISettings
{
    class SDBCompressionExt : Optimizations
    {
        public SDBCompressionExt(ProjectData projectData) : base(projectData)
        {
            if (projectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Count == 0)
            {
                formats = FunctionsInterfaces.GetDBSaveFormats();
                foreach (var ext in formats)
                {
                    projectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add(ext.Description);
                }

                //projectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("XML (none)");
                //projectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Gzip (cmx)");
                //projectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Deflate (cmz)");
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
                Properties.Settings.Default.DBCompressionExt = projectData.Main.Settings.THOptionDBCompressionExtComboBox.SelectedItem.ToString();
            }
            else
            {
                Properties.Settings.Default.DBCompressionExt = IsExistsInFormats(projectData.BufferValueString) ? projectData.BufferValueString : Default;
                projectData.Main.Settings.THOptionDBCompressionExtComboBox.SelectedItem = Properties.Settings.Default.DBCompressionExt;
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
            return projectData.Main.Settings.THOptionDBCompressionExtComboBox.Name;
        }
    }
}
