using TranslationHelper.Data;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.HardFixes
{
    class FixForRPGMAkerQuotationInSomeStrings : HardFixesBase
    {
        public FixForRPGMAkerQuotationInSomeStrings(THDataWork thDataWork) : base(thDataWork)
        {
        }

        protected override bool Apply()
        {
            var NewtransValue = SelectedRow[ColumnIndexTranslation] + "";
            var ret = false;
            //в оригинале " на начале и конце, а в переводе есть также " в середине, что может быть воспринято игрой как ошибка
            //также фикс, когда в оригинале кавычки в начале и конце, а в переводе нет в начале или конце
            bool cvalueStartsWith = NewtransValue.StartsWith("\"");
            bool cvalueEndsWith = NewtransValue.EndsWith("\"");
            var origValue = SelectedRow[ColumnIndexOriginal] as string;
            if (
                 //если оригинал начинается и кончается на ", а в переводе " отсутствует на начале или конце
                 (origValue.StartsWith("\"") && origValue.EndsWith("\"") && (!cvalueStartsWith || !cvalueEndsWith))
                 ||
                 //если перевод начинается и кончается на " и также " есть в где-то середине и количество кавычек не равно
                 (cvalueStartsWith && cvalueEndsWith && NewtransValue.Length > 2
                 && FunctionsString.IsStringAContainsStringB(NewtransValue.Remove(NewtransValue.Length - 1, 1).Remove(0, 1), "\"")
                 //это, чтобы только когда количество кавычек не равно количеству в оригинале
                 && FunctionsString.GetCountOfTheSymbolInStringAandBIsEqual(origValue, NewtransValue, "\"", "\"")))
            {
                NewtransValue = "\"" +
                    NewtransValue
                    .Replace("\"", string.Empty)
                    + "\""
                    ;
                ret = true;
            }
            else
            {
                //rpgmaker mv string will broke script if starts\ends with "'" and contains another "'" in middle
                //в оригинале  ' на начале и конце, а в переводе есть также ' в середине, что может быть воспринято игрой как ошибка, по крайней мере в MV
                cvalueStartsWith = NewtransValue.StartsWith("'");
                cvalueEndsWith = NewtransValue.EndsWith("'");
                if (
                //если оригинал начинается и кончается на ', а в переводе ' отсутствует на начале или конце
                (origValue.StartsWith("'") && origValue.EndsWith("'") && (!cvalueStartsWith || !cvalueEndsWith))
                ||
                //если перевод начинается и кончается на ' и также ' есть в где-то середине
                (cvalueStartsWith && cvalueEndsWith && NewtransValue.Length > 2
                 //&& FunctionsString.IsStringAContainsStringB(NewtransValue.Remove(NewtransValue.Length - 1, 1).Remove(0, 1), "'")
                 //это, чтобы только когда количество ' не равно количеству в оригинале
                 && !FunctionsString.GetCountOfTheSymbolInStringAandBIsEqual(origValue, NewtransValue, "'", "'")))
                {
                    NewtransValue = "'" +
                        NewtransValue
                        .Replace("do n't", "dont")
                        .Replace("don't", "dont")
                        .Replace("n’t", "not")
                        .Replace("'ve", " have")
                        .Replace("I'm", "I am")
                        .Replace("t's", "t is")
                        .Replace("'s", "s")
                        .Replace("'", string.Empty)
                        + "'"
                        ;
                    ret = true;
                }
            }

            if (ret)
            {
                SelectedRow[ColumnIndexTranslation] = NewtransValue;
            }
            return ret;
        }
    }
}
