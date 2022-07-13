using System.Globalization;
using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SDBAutosaveTimeout : Tools
    {
        public SDBAutosaveTimeout()
        {
        }

        internal override string Key => "DBAutosaveTimeout";

        internal override string Default => DefaultInt + string.Empty;
        internal override int DefaultInt => 300;

        object SObject { get => AppData.Settings.SettingsAutosaveTimeoutValueTextBox; }

        static int SVar
        {
            get => Properties.Settings.Default.DBAutoSaveTimeout;
            set => Properties.Settings.Default.DBAutoSaveTimeout = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = CheckAndSetValue();
            }
            else
            {
                SVar = int.TryParse(AppData.BufferValueString, out int result) ? result : DefaultInt;
                (SObject as System.Windows.Forms.TextBox).Text = SVar + string.Empty;
            }
        }

        int CheckAndSetValue()
        {
            if (System.Text.RegularExpressions.Regex.IsMatch((SObject as System.Windows.Forms.TextBox).Text, "^[0-9]{1,4}$") && int.Parse((SObject as System.Windows.Forms.TextBox).Text, CultureInfo.InvariantCulture) <= 9999)
            {
                int newvalue = int.Parse((SObject as System.Windows.Forms.TextBox).Text, CultureInfo.InvariantCulture);
                //Properties.Settings.Default.THOptionLineCharLimit = newvalue;
                return newvalue;
            }
            else
            {
                return DefaultInt;
            }
        }

        internal override string Get()
        {
            return SVar + string.Empty;
        }

        internal override string ID()
        {
            return (SObject as System.Windows.Forms.TextBox).Name;
        }
    }
}
