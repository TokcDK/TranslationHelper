using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TranslationHelper.Functions.FileElementsFunctions.Row.ExtractedParser;
using TranslationHelper.Functions.FileElementsFunctions.Row.OnlineTranslate;

namespace TranslationHelper.Functions.FileElementsFunctions.Row.StringCaseMorph
{
    /// <summary>
    /// Applies a case variant to a row's translation, going through the values extracted from it by
    /// the translation rules so only the translatable parts change.
    /// <para>
    /// The transformation itself is <see cref="CaseMorpher"/>; this class only decides which values to
    /// feed it and which variant to use, which is why a concrete operation is one line long.
    /// </para>
    /// </summary>
    abstract class StringCaseMorphBase : ExtractedByTranslationRulesParserRowBase
    {
        static string Animations { get => "Animations"; }

        /// <summary>
        /// Special translation for Animations table in rpgmaker projects.
        /// Maybe here will be better to make project specific string case morph from Project.StringCaseMorph()
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckAnims(TableData tableData)
        {
            _isAnimations = tableData.SelectedTable.TableName == Animations;
        }

        protected override Task ActionsPreRowsApply(TableData tableData)
        {
            if (!IsAll && !IsTable)
            {
                CheckAnims(tableData);
            }

            return Task.CompletedTask;
        }

        protected override Task ActionsPreTableApply(TableData tableData)
        {
            CheckAnims(tableData);

            return Task.CompletedTask;
        }

        /// <summary>
        /// 0=lowercase,1=Uppercase,2=UPPERCASE
        /// </summary>
        protected abstract CaseMorphVariant Variant { get; }

        /// <summary>
        /// if table name is 'Animations'
        /// <para>
        /// NOTE: nothing reads this at the moment. It is what is left of the Animations special case
        /// ("effect1/effect2" -> "Effect1/Effect2") that the previous implementation had and the
        /// rewrite dropped; the two hooks above exist only to keep it up to date. Left in place
        /// deliberately so the intent is not lost — see the change document.
        /// </para>
        /// </summary>
        bool _isAnimations = false;

        protected override string ActionWithExtractedTranslation(ExtractRegexValueInfo extractedOrigValueInfo, ExtractRegexValueInfo extractedTransValueInfo)
        {
            return CaseMorpher.Change(extractedOrigValueInfo.Original, extractedTransValueInfo.Original, Variant, IsExtracted);
        }

        protected override string ActionWithOriginalIfNoExtracted(string original, string translation)
        {
            return CaseMorpher.Change(original, translation, Variant, IsExtracted);
        }
    }
}
