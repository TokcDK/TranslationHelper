namespace FormatBase
{
    /// <summary>
    /// Helper methods for common line parsing operations
    /// </summary>
    public static class LineParsingHelper
    {
        /// <summary>
        /// Check if a line should be skipped (is a command or comment)
        /// </summary>
        /// <param name="line">The line to check</param>
        /// <returns>True if the line is a command (starts with '@') or comment (starts with '#')</returns>
        public static bool IsCommandOrComment(string? line)
        {
            if (line == null) return false;
            return line.StartsWith('@') || line.StartsWith('#');
        }
    }
}
