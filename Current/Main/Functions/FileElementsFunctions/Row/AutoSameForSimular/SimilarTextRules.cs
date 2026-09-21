using System.Text.RegularExpressions;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.AutoSameForSimular
{
    /// <summary>
    /// The text rules behind "the same translation for a similar original": which characters make two
    /// originals similar, and how the translation of one is carried over to the other.
    /// <para>
    /// Pure: nothing here knows about projects, tables, threads or the grid. It takes strings and
    /// returns strings, which is what makes the rule itself — "книга 1" and "книга 2" are the same
    /// line but for one number, so "book 1" carries over as "book 2" — checkable on its own instead
    /// of only through a whole-project scan.
    /// </para>
    /// <para>
    /// "Number or symbol" means a run of digits, or one of the punctuation marks that a translation
    /// is expected to keep: quotes and the bracketing marks at the start and the end of a line, in
    /// their Latin, Japanese and full-width forms. A line is recognised as similar when removing
    /// those leaves the same text, the same number of them, in the same places.
    /// </para>
    /// </summary>
    internal static class SimilarTextRules
    {
        /// <summary>
        /// Matches every number or symbol of a line, wherever it is.
        /// </summary>
        private static readonly Regex NumbersOrSymbols = new Regex(GetSimilarityPattern(), RegexOptions.Compiled);

        /// <summary>
        /// Matches a run of digits. Separate from <see cref="NumbersOrSymbols"/> because a Japanese
        /// original and its translation do not have to keep the same number of digit runs.
        /// </summary>
        private static readonly Regex DigitRuns = new Regex(@"\d+", RegexOptions.Compiled);

        /// <summary>
        /// Every number or symbol of <paramref name="value"/>, in order. Positions are positions in
        /// the string that was passed in, so the matches can be used to rewrite it.
        /// </summary>
        internal static MatchCollection Matches(string value) => NumbersOrSymbols.Matches(value ?? string.Empty);

        /// <summary>
        /// How many runs of digits <paramref name="value"/> holds.
        /// </summary>
        internal static int DigitRunCount(string value) => DigitRuns.Matches(value ?? string.Empty).Count;

        /// <summary>
        /// <paramref name="value"/> with every number and symbol removed, which is the text two
        /// similar lines have in common.
        /// </summary>
        internal static string WithoutMatches(string value) => NumbersOrSymbols.Replace(value ?? string.Empty, string.Empty);

        /// <summary>
        /// Whether two originals are the same line but for their numbers and symbols: the same text
        /// once those are removed, the same number of them, and in the same places.
        /// </summary>
        /// <param name="inputOriginalWithoutMatches">
        /// The original being spread from, already stripped with <see cref="WithoutMatches"/>. Passed
        /// in rather than stripped here because the same original is compared against every row of
        /// the project.
        /// </param>
        /// <param name="inputMatches"><see cref="Matches"/> of the same original.</param>
        /// <param name="targetOriginal">The original being compared to it.</param>
        /// <param name="targetMatches"><see cref="Matches"/> of that one.</param>
        internal static bool AreSimilar(
            string inputOriginalWithoutMatches, MatchCollection inputMatches,
            string targetOriginal, MatchCollection targetMatches)
        {
            // A different count means a different line even when the text around the numbers matches,
            // and it is also what keeps the substitution below from running out of pairs.
            if (inputMatches.Count != targetMatches.Count) return false;

            if (inputOriginalWithoutMatches != WithoutMatches(targetOriginal)) return false;

            return AreInIdenticalPlaces(inputMatches, targetMatches);
        }

        /// <summary>
        /// Builds the translation of a similar line out of the translation of the line being spread
        /// from: every number or symbol of that translation is replaced by the one the target line
        /// has in the same place. "book 1" for "книга 1" becomes "book 2" for "книга 2".
        /// <para>
        /// The replacements walk from the last match to the first, so replacing one does not move the
        /// positions of the ones still to come.
        /// </para>
        /// </summary>
        /// <param name="inputTranslation">The translation being spread from.</param>
        /// <param name="inputTranslationMatches"><see cref="Matches"/> of it.</param>
        /// <param name="targetOriginalMatches"><see cref="Matches"/> of the original it is carried over to.</param>
        /// <returns>
        /// The translation of the target line, or <paramref name="inputTranslation"/> unchanged when
        /// the two sides do not have a match for every match.
        /// </returns>
        internal static string TranslateLike(
            string inputTranslation, MatchCollection inputTranslationMatches, MatchCollection targetOriginalMatches)
        {
            if (inputTranslation == null) return null;

            // Only meaningful one-to-one: without a pair for every match there is nothing to
            // substitute along, and indexing the shorter side would throw.
            if (inputTranslationMatches.Count != targetOriginalMatches.Count) return inputTranslation;

            var result = inputTranslation;

            for (int matchIndex = inputTranslationMatches.Count - 1; matchIndex >= 0; matchIndex--)
            {
                var from = inputTranslationMatches[matchIndex];
                var to = targetOriginalMatches[matchIndex];

                // A mark that is already the same is left alone: "!" stays "!", "?" stays "?",
                // and only a real difference such as "1" against "2" is substituted.
                if (from.Value == to.Value) continue;

                result = result
                    .Remove(from.Index, from.Length)
                    .Insert(from.Index, to.Value);
            }

            return result;
        }

        /// <summary>
        /// Whether every match of <paramref name="first"/> sits at the same place, relative to the
        /// text between the matches, as the match of <paramref name="second"/> with the same index.
        /// <para>
        /// Comparing the raw positions would reject a pair whose matches differ in length — "1" and
        /// "100" are one match each but do not start at the same offset — so each position is
        /// compared after subtracting the length of the matches seen so far.
        /// </para>
        /// </summary>
        private static bool AreInIdenticalPlaces(MatchCollection first, MatchCollection second)
        {
            // The two sides are compared pairwise below, so a count that does not line up is a "no"
            // rather than something to read past the end of.
            if (first.Count != second.Count) return false;

            int lengthBeforeFirst = 0;
            int lengthBeforeSecond = 0;

            for (int matchIndex = 0; matchIndex < first.Count; matchIndex++)
            {
                if (first[matchIndex].Index - lengthBeforeFirst != second[matchIndex].Index - lengthBeforeSecond)
                    return false;

                lengthBeforeFirst += first[matchIndex].Length;
                lengthBeforeSecond += second[matchIndex].Length;
            }

            return true;
        }

        /// <summary>
        /// The pattern of "a number or a symbol": a run of digits anywhere, or one of the punctuation
        /// marks a line may carry at its start or at its end.
        /// </summary>
        private static string GetSimilarityPattern()
        {
            //http://www.cyberforum.ru/csharp-beginners/thread244709.html
            const string quotationMark = "\"";
            const string latinSymbolsStart = @"\d|\!|\?|\.|\[";
            const string latinSymbolsEnd = @"\d|\!|\?|\.|\]";
            const string japaneseSymbolsStart = @"！|？|。|【|「|『|〝";
            const string japaneseSymbolsEnd = @"！|？|。|】|」|』|〟";
            const string digitsAnywhere = @"\d+";

            string symbolsAtStart = "(^(" + japaneseSymbolsStart + "|" + quotationMark + "|" + latinSymbolsStart + ")+)";
            string symbolsAtEnd = "((" + latinSymbolsEnd + "|" + quotationMark + "|" + japaneseSymbolsEnd + ")+$)";

            return symbolsAtStart + "|" + digitsAnywhere + "|" + symbolsAtEnd;
        }
    }
}
