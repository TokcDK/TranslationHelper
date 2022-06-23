using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SFullComprasionDBload : General
    {
        public SFullComprasionDBload()
        {
        }

        internal override string Key => "FullComprasionDBload";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => false;

        object SObject { get => AppData.Settings.THOptionFullComprasionDBloadCheckBox; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.IsFullComprasionDBloadEnabled;
            set => TranslationHelper.Properties.Settings.Default.IsFullComprasionDBloadEnabled = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = AppData.Settings.THOptionDBCompressionCheckBox.Checked;
            }
            else
            {
                SVar = bool.TryParse(AppData.BufferValueString, out bool result) ? result : DefaultBool;
                (SObject as System.Windows.Forms.CheckBox).Checked = SVar;
            }
        }

        internal override string Get()
        {
            return SVar + string.Empty;
        }

        internal override string ID()
        {
            return (SObject as System.Windows.Forms.CheckBox).Name;
        }
    }
}
