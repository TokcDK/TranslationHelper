namespace TranslationHelper.Formats.RPGMMV.JS
{
    interface IUseJsLocationInfo
    {
        /// <summary>
        /// Name of js file
        /// </summary>
        string JsName { get; }

        /// <summary>
        /// folder where the js file is located
        /// </summary>
        string JsSubfolder { get; }
    }
}
