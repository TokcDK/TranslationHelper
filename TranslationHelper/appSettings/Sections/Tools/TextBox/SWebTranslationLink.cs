using TranslationHelper.Data;

namespace TranslationHelper.INISettings
{
    class SWebTranslationLink : Tools
    {
        public SWebTranslationLink(ProjectData projectData) : base(projectData)
        {
        }

        internal override string Key => "WebTranslationLink";

        internal override string Default => "https://translate.google.com/?ie=UTF-8&op=translate&sl={from}&tl={to}&text={text}";

        object SObject { get => projectData.Main.Settings.THSettingsWebTranslationLinkTextBox; }

        static string SVar
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
                SVar = projectData.BufferValueString;
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
