namespace TranslationHelper.Formats.Abstractions
{
    /// <summary>
    /// The owner of a format: the environment a format runs in.
    /// <para>
    /// This is the whole of what a format is allowed to know about the project that drives it.
    /// The contract is declared here, next to its consumer, so the <c>Formats</c> layer never has
    /// to reference the <c>Projects</c> layer; the dependency points one way only. The project
    /// implements this interface, and a format receives it through its constructor.
    /// </para>
    /// </summary>
    public interface IFormatHost : ITranslationStore
    {
        /// <summary>
        /// Gets the directory of the selected game. Formats read the files they parse from here.
        /// </summary>
        string SelectedGameDir { get; }

        /// <summary>
        /// Gets the directory the project keeps its working files in.
        /// </summary>
        string ProjectWorkDir { get; }

        /// <summary>
        /// Gets the directory the currently opened files were read from.
        /// </summary>
        string OpenedFilesDir { get; }

        /// <summary>
        /// Gets a value indicating whether the path of a file inside <see cref="SelectedGameDir"/>
        /// is part of its table name.
        /// </summary>
        bool SubpathInTableName { get; }

        /// <summary>
        /// Gets a value indicating whether a file is written back to the exact path it was opened
        /// from instead of to the project work directory.
        /// </summary>
        bool IsSaveToSourceFile { get; }

        /// <summary>
        /// Applies the project's own rules for deciding whether a string is worth translating.
        /// </summary>
        /// <param name="str">The string to clean.</param>
        /// <returns>The cleaned string.</returns>
        string CleanStringForCheck(string str);
    }
}
