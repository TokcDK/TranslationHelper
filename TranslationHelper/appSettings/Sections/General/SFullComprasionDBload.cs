using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SFullComprasionDBload : General
    {
        public SFullComprasionDBload(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Key => "FullComprasionDBload";

        internal override string Default => DefaultBool.ToString();
        internal override bool DefaultBool => false;

        object SObject { get => thDataWork.Main.Settings.THOptionFullComprasionDBloadCheckBox; }

        bool SVar
        {
            get => TranslationHelper.Properties.Settings.Default.IsFullComprasionDBloadEnabled;
            set => TranslationHelper.Properties.Settings.Default.IsFullComprasionDBloadEnabled = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = thDataWork.Main.Settings.THOptionDBCompressionCheckBox.Checked;
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
