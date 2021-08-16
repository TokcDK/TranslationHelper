using System.Collections.Generic;
using TranslationHelper.Data;
using TranslationHelper.Functions;
using TranslationHelper.Functions.DBSaveFormats;

namespace TranslationHelper.INISettings
{
    class SdbCompressionExt : Optimizations
    {
        public SdbCompressionExt()
        {
            if (ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Count == 0)
            {
                _formats = FunctionsInterfaces.GetDbSaveFormats();
                foreach (var ext in _formats)
                {
                    ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add(ext.Description);
                }

                //ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("XML (none)");
                //ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Gzip (cmx)");
                //ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Deflate (cmz)");
            }
        }

        List<IDbSave> _formats;
        readonly IDbSave _defaultformat = new Xml();

        internal override string Key => "DBCompressionExt";

        internal override string Default => _defaultformat.Description;

        internal override void Set(bool setObject = false)
        {
            if (!setObject)
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
            if(_formats == null)
            {
                _formats = FunctionsInterfaces.GetDbSaveFormats();
            }
            foreach (var format in _formats)
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

        internal override string Id()
        {
            return ProjectData.Main.Settings.THOptionDBCompressionExtComboBox.Name;
        }
    }
}
