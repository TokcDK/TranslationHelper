using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SDBCompressionExt : Optimizations
    {
        public SDBCompressionExt(THDataWork thDataWork) : base(thDataWork)
        {
            if (thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.Items.Count == 0)
            {
                thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("XML (none)");
                thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Gzip(cmx)");
                thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.Items.Add("Deflate(cmz)");
            }
        }

        internal override string Key => "DBCompressionExt";

        internal override string Default => "XML (none)";

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                TranslationHelper.Properties.Settings.Default.DBCompressionExt = thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.SelectedItem.ToString();
            }
            else
            {
                TranslationHelper.Properties.Settings.Default.DBCompressionExt = thDataWork.BufferValueString;
                thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.SelectedItem = TranslationHelper.Properties.Settings.Default.DBCompressionExt;
            }
        }

        internal override string Get()
        {
            return TranslationHelper.Properties.Settings.Default.DBCompressionExt;
        }

        internal override string ID()
        {
            return thDataWork.Main.Settings.THOptionDBCompressionExtComboBox.Name;
        }
    }
}
