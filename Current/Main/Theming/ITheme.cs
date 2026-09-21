using System.Drawing;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// What a theme is: the colours one look of the application is painted with, named after the role
    /// each one plays rather than after the control that happens to use it.
    /// <para>
    /// This is the contract, and it is deliberately only data. It knows no WinForms type, so the
    /// control stylers, the files list and the menu renderer can all read the same theme without
    /// agreeing on anything except these names. How a theme is applied is not here; that is what
    /// <see cref="IControlStyler"/> is for. Adding a theme therefore means adding a class that
    /// implements this one and registering it with <see cref="ThemeManager"/>, and touching nothing
    /// that already exists.
    /// </para>
    /// <para>
    /// Every member is read only. A theme is handed out complete and shared by every window, so a
    /// theme that could be written to after it had been published would let any caller change the look
    /// of the whole application from an arbitrary place.
    /// </para>
    /// </summary>
    internal interface ITheme
    {
        /// <summary>
        /// Name the setting stores and the settings form offers. It is also the lookup key, so it is
        /// the one thing about a theme that must stay stable once it has been released.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// True for a theme that paints light text on dark surfaces.
        /// <para>
        /// Needed by the few controls whose appearance is not fully described by colours: a check box,
        /// a radio button and a combo box each have a part that the framework draws, and only some of
        /// the styles that draw them can be recoloured. Those parts pick their style from this flag
        /// instead of from a colour.
        /// </para>
        /// </summary>
        bool IsDark { get; }

        // Surfaces: forms, panels, table layouts, split containers, tab pages, group boxes, user controls.

        /// <summary>
        /// Background of a window itself. Separate from <see cref="SurfaceBack"/> so that a dark theme
        /// can put a lighter container on a darker window, which is what makes a panel visible as a
        /// panel rather than as an invisible region of the window.
        /// </summary>
        Color FormBack { get; }

        /// <summary>Background of a container and of the labels sitting on it.</summary>
        Color SurfaceBack { get; }

        /// <summary>Ordinary text on a surface: labels, check box captions, group box captions.</summary>
        Color SurfaceText { get; }

        /// <summary>One pixel outlines: panel borders and the outline of an unselected tab.</summary>
        Color Border { get; }

        /// <summary>Split container splitter bars, which otherwise keep the system colour.</summary>
        Color Splitter { get; }

        // Editors: text boxes, rich text boxes, list boxes and the text part of a combo box.

        /// <summary>Background of a control the user types into or picks from.</summary>
        Color EditorBack { get; }

        Color EditorText { get; }

        /// <summary>Used where an editor's selection can be recoloured, such as the files list.</summary>
        Color EditorSelectionBack { get; }

        Color EditorSelectionText { get; }

        // Buttons.

        Color ButtonBack { get; }

        Color ButtonText { get; }

        /// <summary>
        /// Background of a button the pointer is over, and of one being pressed. A flat button keeps
        /// the framework's own light colours for these two unless they are set, so without them a dark
        /// button would flash a near white block under the pointer.
        /// </summary>
        Color ButtonHoverBack { get; }

        Color ButtonPressedBack { get; }

        /// <summary>Link labels and other clickable text.</summary>
        Color LinkText { get; }

        // Menus: the menu strip, the context menus and their drop-downs.

        /// <summary>Background of the menu strip and of a drop-down.</summary>
        Color MenuBack { get; }

        Color MenuText { get; }

        /// <summary>Background of the item under the pointer and of an open top level menu.</summary>
        Color MenuHighlightBack { get; }

        Color MenuHighlightText { get; }

        Color MenuBorder { get; }

        /// <summary>The gutter left of the icons and check marks of a drop-down.</summary>
        Color MenuImageMargin { get; }

        // Data grid.

        /// <summary>Background of a cell and of the empty area below the last row.</summary>
        Color GridBack { get; }

        /// <summary>Background of every second row, which is what makes a wide grid readable.</summary>
        Color GridAlternateBack { get; }

        Color GridText { get; }

        /// <summary>The lines between cells, and between the row headers and the first column.</summary>
        Color GridLines { get; }

        Color GridHeaderBack { get; }

        Color GridHeaderText { get; }

        Color GridSelectionBack { get; }

        Color GridSelectionText { get; }

        // Files list, which the application draws itself.

        Color ListRowBack { get; }

        Color ListRowAlternateBack { get; }

        /// <summary>Row of a file whose translation is finished.</summary>
        Color ListRowBackComplete { get; }

        Color ListRowAlternateBackComplete { get; }

        Color ListRowText { get; }

        Color ListRowSelectedBack { get; }

        Color ListRowSelectedText { get; }

        // Tab strips, which the application draws itself.

        /// <summary>Background of a tab that is not the selected one.</summary>
        Color TabBack { get; }

        Color TabText { get; }

        /// <summary>
        /// Background of the selected tab. Chosen to match <see cref="SurfaceBack"/> so that the
        /// selected tab and the page under it read as one surface, which is what tells the user
        /// which page is open.
        /// </summary>
        Color TabSelectedBack { get; }

        Color TabSelectedText { get; }

        /// <summary>Caption of a control the user cannot use, and of a disabled menu item.</summary>
        Color DisabledText { get; }
    }
}
