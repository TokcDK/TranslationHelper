﻿using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SIgnoreOrigEqualTransLines : Optimizations
    {
        public SIgnoreOrigEqualTransLines() : base()
        {
        }

        internal override string Key => "IgnoreOrigEqualTransLines";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => ProjectData.Main.Settings.THOptionIgnoreOrigEqualTransLinesCheckBox; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.IgnoreOrigEqualTransLines;
            set => TranslationHelper.Properties.Settings.Default.IgnoreOrigEqualTransLines = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
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

        internal override string ID()
        {
            return (SObject as System.Windows.Forms.CheckBox).Name;
        }
    }
}
