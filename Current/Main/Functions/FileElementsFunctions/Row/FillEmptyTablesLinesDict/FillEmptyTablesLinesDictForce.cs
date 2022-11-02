namespace TranslationHelper.Functions.FileElementsFunctions.Row.FillEmptyTablesLinesDict
{
    /// <summary>
    /// Fille TableLines dictionary with original-translation pairs. Force variant with ignore savemode or disabled donotdups
    /// </summary>
    class FillEmptyTablesLinesDictForce : FillEmptyTablesLinesDictBase
    {
        protected override bool ForceRun => true;
    }
}
