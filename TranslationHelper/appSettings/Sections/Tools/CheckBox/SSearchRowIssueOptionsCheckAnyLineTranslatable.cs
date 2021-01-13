﻿using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SSearchRowIssueOptionsCheckAnyLineTranslatable : Tools
    {
        public SSearchRowIssueOptionsCheckAnyLineTranslatable(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Key => "SearchRowIssueOptionsCheckAnyLineTranslatable";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => thDataWork.Main.Settings.cbSearchRowIssueOptionsCheckAnyLineTranslatable; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.SearchRowIssueOptionsCheckAnyLineTranslatable;
            set => TranslationHelper.Properties.Settings.Default.SearchRowIssueOptionsCheckAnyLineTranslatable = value;
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