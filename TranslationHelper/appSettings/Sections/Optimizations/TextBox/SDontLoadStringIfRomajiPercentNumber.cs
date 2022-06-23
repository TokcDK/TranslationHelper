using System.Globalization;
using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SDontLoadStringIfRomajiPercentNumber : Optimizations
    {
        public SDontLoadStringIfRomajiPercentNumber()
        {
        }

        internal override string Key => "DontLoadStringIfRomajiPercentNumber";

        internal override string Default => DefaultInt + string.Empty;
        internal override int DefaultInt => 90;

        object SObject { get => AppData.Settings.DontLoadStringIfRomajiPercentNumberTextBox; }

        static int SVar
        {
            get => TranslationHelper.Properties.Settings.Default.DontLoadStringIfRomajiPercentNumber;
            set => TranslationHelper.Properties.Settings.Default.DontLoadStringIfRomajiPercentNumber = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = CheckAndSet();
            }
            else
            {
                SVar = int.TryParse(AppData.BufferValueString, out int result) ? result : DefaultInt;
                (SObject as System.Windows.Forms.TextBox).Text = SVar + string.Empty;
            }
        }

        private int CheckAndSet()
        {
            if (System.Text.RegularExpressions.Regex.IsMatch((SObject as System.Windows.Forms.TextBox).Text, "^[0-9]{1,3}$") && int.Parse((SObject as System.Windows.Forms.TextBox).Text, CultureInfo.InvariantCulture) <= 100)
            {
                int newvalue = int.Parse((SObject as System.Windows.Forms.TextBox).Text, CultureInfo.InvariantCulture);
                //Properties.Settings.Default.DontLoadStringIfRomajiPercentNumber = newvalue;
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
