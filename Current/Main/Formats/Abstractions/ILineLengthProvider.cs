namespace TranslationHelper.Formats.Abstractions
{
    /// <summary>
    /// Implemented by a host that knows how many characters of text the game can display on one
    /// line.
    /// <para>
    /// A format that has to wrap long text asks for this capability instead of casting its host to
    /// the concrete project type, which is what used to tie the <c>Formats</c> layer to a specific
    /// project class. Only the projects that have such a limit implement it.
    /// </para>
    /// </summary>
    public interface ILineLengthProvider
    {
        /// <summary>
        /// Gets the maximum number of characters the game can display on one line.
        /// </summary>
        int MaxLineLength { get; }
    }
}
