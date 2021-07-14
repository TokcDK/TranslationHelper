﻿using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SSearchRowIssueOptionsCheckNonRomaji : Tools
    {
        public SSearchRowIssueOptionsCheckNonRomaji(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Key => "SearchRowIssueOptionsCheckNonRomaji";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => projectData.Main.Settings.cbSearchRowIssueOptionsCheckNonRomaji; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.SearchRowIssueOptionsCheckNonRomaji;
            set => TranslationHelper.Properties.Settings.Default.SearchRowIssueOptionsCheckNonRomaji = value;
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
