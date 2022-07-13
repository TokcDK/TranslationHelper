using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SSearchRowIssueOptionsCheckAnyLineTranslatable : Tools
    {
        public SSearchRowIssueOptionsCheckAnyLineTranslatable()
        {
        }

        internal override string Key => "SearchRowIssueOptionsCheckAnyLineTranslatable";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => AppData.Settings.cbSearchRowIssueOptionsCheckAnyLineTranslatable; }

        static bool SVar
        {
            get => AppSettings.SearchRowIssueOptionsCheckAnyLineTranslatable;
            set => AppSettings.SearchRowIssueOptionsCheckAnyLineTranslatable = value;
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
