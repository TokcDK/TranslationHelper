using TranslationHelper.Formats.Abstractions;

namespace TranslationHelper.Formats.RPGMMV.JS
{
    class ZZZOtherJS : JSQuotedStringsBase
    {
        public ZZZOtherJS(IFormatHost host) : base(host)
        {
        }

        public override string JSName => "TEMPLATE";
    }
}
