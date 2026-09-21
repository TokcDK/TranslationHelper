using System.Drawing;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// A dark theme built from one grey ramp so that nesting reads correctly: a container is lighter
    /// than the window behind it, a tab strip is darker than the page it belongs to, and text is the
    /// same off-white everywhere so that nothing has to be squinted at.
    /// <para>
    /// Nothing here is derived from the system, because the point of the theme is to ignore the
    /// system's light colours. The values are therefore written down, and this file is where the dark
    /// look is decided.
    /// </para>
    /// </summary>
    internal sealed class DarkTheme : ITheme
    {
        public string Name => ThemeManager.DarkName;

        public bool IsDark => true;

        // The window is the darkest surface, so that a container inside it stands out by being lighter.

        public Color FormBack { get; } = Color.FromArgb(32, 32, 32);

        public Color SurfaceBack { get; } = Color.FromArgb(45, 45, 48);

        public Color SurfaceText { get; } = Color.FromArgb(240, 240, 240);

        public Color Border { get; } = Color.FromArgb(63, 63, 70);

        public Color Splitter { get; } = Color.FromArgb(45, 45, 48);

        // Editors are darker than the surface they sit on, which is what makes a text box read as a
        // hole to type into rather than as another panel.

        public Color EditorBack { get; } = Color.FromArgb(30, 30, 30);

        public Color EditorText { get; } = Color.FromArgb(240, 240, 240);

        public Color EditorSelectionBack { get; } = Color.FromArgb(38, 79, 120);

        public Color EditorSelectionText { get; } = Color.White;

        public Color ButtonBack { get; } = Color.FromArgb(63, 63, 70);

        public Color ButtonText { get; } = Color.White;

        // Both are set, because a flat button paints its own highlight: left to the framework it would
        // flash a near white block under the pointer on a dark window.
        public Color ButtonHoverBack { get; } = Color.FromArgb(80, 80, 85);

        public Color ButtonPressedBack { get; } = Color.FromArgb(95, 95, 100);

        // A link has to stay recognisable as a link on a dark surface, which rules out the system's
        // dark blue.
        public Color LinkText { get; } = Color.FromArgb(78, 161, 255);

        public Color MenuBack { get; } = Color.FromArgb(45, 45, 48);

        public Color MenuText { get; } = Color.FromArgb(240, 240, 240);

        public Color MenuHighlightBack { get; } = Color.FromArgb(62, 62, 66);

        public Color MenuHighlightText { get; } = Color.White;

        public Color MenuBorder { get; } = Color.FromArgb(63, 63, 70);

        public Color MenuImageMargin { get; } = Color.FromArgb(37, 37, 38);

        public Color GridBack { get; } = Color.FromArgb(37, 37, 38);

        public Color GridAlternateBack { get; } = Color.FromArgb(45, 45, 48);

        public Color GridText { get; } = Color.FromArgb(241, 241, 241);

        public Color GridLines { get; } = Color.FromArgb(63, 63, 70);

        public Color GridHeaderBack { get; } = Color.FromArgb(51, 51, 55);

        public Color GridHeaderText { get; } = Color.FromArgb(241, 241, 241);

        public Color GridSelectionBack { get; } = Color.FromArgb(38, 79, 120);

        public Color GridSelectionText { get; } = Color.White;

        // Dark green instead of the light theme's pale green: the tint still says "finished" without
        // turning into a bright block in a dark list.
        public Color ListRowBack { get; } = Color.FromArgb(30, 30, 30);

        public Color ListRowAlternateBack { get; } = Color.FromArgb(38, 38, 38);

        public Color ListRowBackComplete { get; } = Color.FromArgb(30, 58, 34);

        public Color ListRowAlternateBackComplete { get; } = Color.FromArgb(36, 66, 42);

        public Color ListRowText { get; } = Color.FromArgb(240, 240, 240);

        public Color ListRowSelectedBack { get; } = Color.FromArgb(38, 79, 120);

        public Color ListRowSelectedText { get; } = Color.White;

        // The strip is darker than the page, and the selected tab is the colour of the page, so that
        // the open tab reads as part of the page it belongs to.
        public Color TabBack { get; } = Color.FromArgb(37, 37, 38);

        public Color TabText { get; } = Color.FromArgb(176, 176, 176);

        public Color TabSelectedBack { get; } = Color.FromArgb(45, 45, 48);

        public Color TabSelectedText { get; } = Color.FromArgb(240, 240, 240);

        public Color DisabledText { get; } = Color.FromArgb(122, 122, 122);

        public override string ToString()
        {
            return Name;
        }
    }
}
