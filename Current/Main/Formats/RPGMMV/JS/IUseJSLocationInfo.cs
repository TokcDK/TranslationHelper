namespace TranslationHelper.Formats.RPGMMV.JS
{
    interface IUseJSLocationInfo
    {
        /// <summary>
        /// Name of js file
        /// </summary>
        string JSName { get; }

        /// <summary>
        /// folder where the js file is located
        /// </summary>
        string JSSubfolder { get; }
    }
}
