using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats
{
    internal abstract class FormatTxtFileBase : FormatStringBase
    {
        protected FormatTxtFileBase(IFormatHost host) : base(host)
        {
        }

        public override string Extension => ".txt";
    }
}
