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

        object SObject { get => ProjectData.Main.Settings.THOptionDontLoadStringIfRomajiPercentCheckBox; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.DontLoadStringIfRomajiPercent;
            set => TranslationHelper.Properties.Settings.Default.DontLoadStringIfRomajiPercent = value;
        }

        internal override void Set(bool setObject = false)
        {
            if (!setObject)
            {
                SVar = (SObject as System.Windows.Forms.CheckBox).Checked;
            }
            else
            {
                SVar = bool.TryParse(ProjectData.BufferValueString, out bool result) ? result : DefaultBool;
                (SObject as System.Windows.Forms.CheckBox).Checked = SVar;
            }
        }

        internal override string Get()
        {
            return SVar + string.Empty;
        }

        internal override string Id()
        {
            return (SObject as System.Windows.Forms.CheckBox).Name;
        }
    }
}
