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
            if (AppData.Settings.THOptionDBCompressionExtComboBox.Items.Count == 0)
            {
                formats = FunctionsInterfaces.GetDBSaveFormats();
                foreach (var ext in formats)
                {
                    AppData.Settings.THOptionDBCompressionExtComboBox.Items.Add(ext.Description);
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
                AppSettings.DBCompressionExt = AppData.Settings.THOptionDBCompressionExtComboBox.SelectedItem.ToString();
            }
            else
            {
                AppSettings.DBCompressionExt = IsExistsInFormats(AppData.BufferValueString) ? AppData.BufferValueString : Default;
                AppData.Settings.THOptionDBCompressionExtComboBox.SelectedItem = AppSettings.DBCompressionExt;
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
            return AppSettings.DBCompressionExt;
        }

        internal override string ID()
        {
            return AppData.Settings.THOptionDBCompressionExtComboBox.Name;
        }
    }
}
