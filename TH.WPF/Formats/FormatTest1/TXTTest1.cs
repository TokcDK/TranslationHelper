using FormatBase;

namespace FormatTest1
{
    public class TXTTest1 : FormatStringBase
    {
        public override string Extension => ".txt";

        public override string Description => "test format 1";

        protected override ParseLineRetType ParseLine()
        {
            return ParseLineRetType.Continue;
        }
    }
}