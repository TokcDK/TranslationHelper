using System.Drawing;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// The theme the application has always had.
    /// <para>
    /// Its colours come from the system rather than being written down here, so choosing it restores
    /// the system look on a machine whose colours the user has customised, and an installation that
    /// predates the theme setting keeps the appearance it was designed with.
    /// </para>
    /// <para>
    /// The menu colours are read from the framework's own menu renderer for the same reason: a list of
    /// system colours copied out of it could disagree with what that renderer actually paints, and then
    /// switching back to this theme would not quite restore what the application looked like before
    /// themes existed.
    /// </para>
    /// <para>
    /// The four files list colours are the exception, and are written down: the list is drawn by the
    /// application rather than by the system, so these are simply the colours it was drawn with before
    /// themes existed. Keeping them exact is what preserves the "translation finished" tint the user
    /// knows.
    /// </para>
    /// </summary>
    internal sealed class LightTheme : ITheme
    {
        /// <summary>
        /// The colour table the framework's own menu renderer uses.
        /// </summary>
        private static readonly ProfessionalColorTable NativeMenuColors = new ProfessionalColorTable();

        public string Name => ThemeManager.LightName;

        public bool IsDark => false;

        public Color FormBack { get; } = SystemColors.Control;

        public Color SurfaceBack { get; } = SystemColors.Control;

        public Color SurfaceText { get; } = SystemColors.ControlText;

        public Color Border { get; } = SystemColors.ControlDark;

        public Color Splitter { get; } = SystemColors.Control;

        public Color EditorBack { get; } = SystemColors.Window;

        public Color EditorText { get; } = SystemColors.WindowText;

        public Color EditorSelectionBack { get; } = SystemColors.Highlight;

        public Color EditorSelectionText { get; } = SystemColors.HighlightText;

        public Color ButtonBack { get; } = SystemColors.Control;

        public Color ButtonText { get; } = SystemColors.ControlText;

        public Color ButtonHoverBack { get; } = SystemColors.ControlLight;

        public Color ButtonPressedBack { get; } = SystemColors.ControlDark;

        public Color LinkText { get; } = SystemColors.HotTrack;

        public Color MenuBack { get; } = NativeMenuColors.MenuStripGradientBegin;

        public Color MenuText { get; } = SystemColors.ControlText;

        public Color MenuHighlightBack { get; } = NativeMenuColors.MenuItemSelected;

        public Color MenuHighlightText { get; } = SystemColors.HighlightText;

        public Color MenuBorder { get; } = NativeMenuColors.MenuBorder;

        public Color MenuImageMargin { get; } = NativeMenuColors.ImageMarginGradientMiddle;

        public Color GridBack { get; } = SystemColors.Window;

        public Color GridAlternateBack { get; } = SystemColors.Window;

        public Color GridText { get; } = SystemColors.WindowText;

        public Color GridLines { get; } = SystemColors.ControlDark;

        public Color GridHeaderBack { get; } = SystemColors.Control;

        public Color GridHeaderText { get; } = SystemColors.ControlText;

        public Color GridSelectionBack { get; } = SystemColors.Highlight;

        public Color GridSelectionText { get; } = SystemColors.HighlightText;

        public Color ListRowBack { get; } = Color.White;

        public Color ListRowAlternateBack { get; } = Color.FromArgb(235, 240, 235);

        public Color ListRowBackComplete { get; } = Color.FromArgb(235, 255, 235);

        public Color ListRowAlternateBackComplete { get; } = Color.FromArgb(225, 255, 225);

        public Color ListRowText { get; } = Color.Black;

        public Color ListRowSelectedBack { get; } = Color.FromKnownColor(KnownColor.Highlight);

        public Color ListRowSelectedText { get; } = Color.White;

        public Color TabBack { get; } = SystemColors.Control;

        public Color TabText { get; } = SystemColors.ControlText;

        public Color TabSelectedBack { get; } = SystemColors.Control;

        public Color TabSelectedText { get; } = SystemColors.ControlText;

        public Color DisabledText { get; } = SystemColors.GrayText;

        public override string ToString()
        {
            return Name;
        }
    }
}
