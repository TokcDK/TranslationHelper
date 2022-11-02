using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SDontLoadStringIfRomajiPercent : Optimizations
    {
        public SDontLoadStringIfRomajiPercent()
        {
        }

        internal override string Key => "DontLoadStringIfRomajiPercent";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => AppData.Settings.THOptionDontLoadStringIfRomajiPercentCheckBox; }

        static bool SVar
        {
            get => AppSettings.DontLoadStringIfRomajiPercent;
            set => AppSettings.DontLoadStringIfRomajiPercent = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = (SObject as System.Windows.Forms.CheckBox).Checked;
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
