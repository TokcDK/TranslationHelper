using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SAutotranslationForSimular : General
    {
        public SAutotranslationForSimular()
        {
        }

        internal override string Key => "AutotranslationForSimular";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SCheckBox { get => AppData.Main.Settings.THOptionAutotranslationForSimularCheckBox; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.AutotranslationForSimular;
            set => TranslationHelper.Properties.Settings.Default.AutotranslationForSimular = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = (SCheckBox as System.Windows.Forms.CheckBox).Checked;
            }
            else
            {
                SVar = bool.TryParse(AppData.BufferValueString, out bool result) ? result : DefaultBool;
                (SCheckBox as System.Windows.Forms.CheckBox).Checked = SVar;
            }
        }

        internal override string Get()
        {
            return SVar + string.Empty;
        }

        internal override string ID()
        {
            return (SCheckBox as System.Windows.Forms.CheckBox).Name;
        }
    }
}
