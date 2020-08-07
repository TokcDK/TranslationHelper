using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SWebTranslationLink : Tools
    {
        public SWebTranslationLink(THDataWork thDataWork) : base(thDataWork)
        {
        }

        internal override string Key => "WebTranslationLink";

        internal override string Default => "https://translate.google.com/?ie=UTF-8&op=translate&sl=auto&tl=en&text={text}";

        object SObject { get => thDataWork.Main.Settings.THSettingsWebTranslationLinkTextBox; }

        string SVar
        {
            get => TranslationHelper.Properties.Settings.Default.WebTranslationLink;
            set => TranslationHelper.Properties.Settings.Default.WebTranslationLink = value;
        }

        internal override void Set(bool SetObject = false)
        {
            if (!SetObject)
            {
                SVar = (SObject as System.Windows.Forms.TextBox).Text;
            }
            else
            {
                SVar = thDataWork.BufferValueString;
                (SObject as System.Windows.Forms.TextBox).Text = SVar;
            }
        }

        internal override string Get()
        {
            return SVar;
        }

        internal override string ID()
        {
            return (SObject as System.Windows.Forms.TextBox).Name;
        }
    }
}
