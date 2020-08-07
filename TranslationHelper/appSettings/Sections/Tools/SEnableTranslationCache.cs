using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SEnableTranslationCache : Tools
    {
        public SEnableTranslationCache(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Key => "EnableTranslationCache";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => thDataWork.Main.Settings.THOptionEnableTranslationCacheCheckBox; }

        bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.EnableTranslationCache;
            set => TranslationHelper.Properties.Settings.Default.EnableTranslationCache = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = (SObject as System.Windows.Forms.CheckBox).Checked;
            }
            else
            {
                SVar = bool.TryParse(thDataWork.BufferValueString, out bool result) ? result : DefaultBool;
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
