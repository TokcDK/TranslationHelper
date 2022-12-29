using FormatBase;

namespace FormatTest1
{
    public class TXTTest1 : FormatStringBase
    {
        public override string Extension => ".txt";

        public override string Description => "test format 1";

        protected override ParseLineRetType ParseLine()
        {
            if (Line!.StartsWith('@')) return ParseLineRetType.Continue; // command
            if (Line.StartsWith('#')) return ParseLineRetType.Continue; // comment

            Add(Line);

            return ParseLineRetType.Continue;
        }
    }
}