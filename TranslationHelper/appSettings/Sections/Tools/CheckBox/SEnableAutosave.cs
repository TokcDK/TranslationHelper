﻿using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SEnableAutosave : Tools
    {
        public SEnableAutosave() : base()
        {
        }

        internal override string Key => "EnableDBAutosave";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => true;

        object SObject { get => ProjectData.Main.Settings.SettingsAutosaveEnabledCheckBox; }

        static bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.EnableDBAutosave;
            set => TranslationHelper.Properties.Settings.Default.EnableDBAutosave = value;
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
