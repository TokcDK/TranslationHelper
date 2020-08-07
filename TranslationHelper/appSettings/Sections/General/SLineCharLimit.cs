﻿using System.Globalization;
using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SLineCharLimit : General
    {
        public SLineCharLimit(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Key => "LineCharLimit";

        internal override string Default => DefaultInt + string.Empty;
        internal override int DefaultInt => 60;

        object SObject { get => thDataWork.Main.Settings.LineCharLimitTextBox; }

        int SVar
        {
            get => TranslationHelper.Properties.Settings.Default.THOptionLineCharLimit;
            set => TranslationHelper.Properties.Settings.Default.THOptionLineCharLimit = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = CheckAndSetValue();
            }
            else
            {
                SVar = int.TryParse(thDataWork.BufferValueString, out int result) ? result : DefaultInt;
                (SObject as System.Windows.Forms.TextBox).Text = SVar + string.Empty;
            }
        }

        int CheckAndSetValue()
        {
            if (System.Text.RegularExpressions.Regex.IsMatch((SObject as System.Windows.Forms.TextBox).Text, "^[0-9]{1,4}$") && int.Parse((SObject as System.Windows.Forms.TextBox).Text, CultureInfo.GetCultureInfo("en-US")) <= 9999)
            {
                int newvalue = int.Parse((SObject as System.Windows.Forms.TextBox).Text, CultureInfo.GetCultureInfo("en-US"));
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
