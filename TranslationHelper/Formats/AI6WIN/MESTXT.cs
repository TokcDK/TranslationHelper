namespace TranslationHelper.Formats.AI6WIN
{
    internal class MESTXT : FormatTxtFileBase
    {
        protected override KeywordActionAfter ParseStringFileLine()
        {
            if (ParseData.Line == "#1-STR_PRIMARY")
            {
                var s = ReadLine();
                s = s.Substring(2, s.Length - 4);
                AddRowData(ref s, "");

                if (SaveFileMode) ParseData.Line = "#1-STR_PRIMARY" + "\r\n" + "[\"" + s + "\"]";
            }

            SaveModeAddLine();

            return KeywordActionAfter.Continue;
        }
    }
}
