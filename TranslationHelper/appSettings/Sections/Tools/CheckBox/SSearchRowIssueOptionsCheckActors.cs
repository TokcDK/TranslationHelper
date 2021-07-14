using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SSearchRowIssueOptionsCheckActors : Tools
    {
        public SSearchRowIssueOptionsCheckActors(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Key => "SearchRowIssueOptionsCheckActors";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => projectData.Main.Settings.cbSearchRowIssueOptionsCheckActors; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.SearchRowIssueOptionsCheckActors;
            set => TranslationHelper.Properties.Settings.Default.SearchRowIssueOptionsCheckActors = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = (SObject as System.Windows.Forms.CheckBox).Checked;
            }
            else
            {
                SVar = bool.TryParse(projectData.BufferValueString, out bool result) ? result : DefaultBool;
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
