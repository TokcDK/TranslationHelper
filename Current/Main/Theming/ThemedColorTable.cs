using System.Drawing;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// The menu colours a <see cref="ToolStripProfessionalRenderer"/> asks for while it paints.
    /// <para>
    /// A menu is not painted from its own <c>BackColor</c>: the framework hands the work to a
    /// renderer, which reads every colour it needs from a table like this one. Overriding the table
    /// is therefore the only way to recolour a menu, and it is what makes the menu strip, the
    /// context menus and every drop-down follow the theme together.
    /// </para>
    /// <para>
    /// The colours are read from the current theme when they are asked for rather than captured when
    /// the table is built, so a table never has to be replaced after a theme change — a menu is
    /// repainted when it opens, and it paints the theme that is current then.
    /// </para>
    /// </summary>
    internal sealed class ThemedColorTable : ProfessionalColorTable
    {
        private static ITheme Palette => ThemeManager.Instance.CurrentTheme;

        public override Color MenuStripGradientBegin => Palette.MenuBack;

        public override Color MenuStripGradientEnd => Palette.MenuBack;

        public override Color ToolStripGradientBegin => Palette.MenuBack;

        public override Color ToolStripGradientMiddle => Palette.MenuBack;

        public override Color ToolStripGradientEnd => Palette.MenuBack;

        public override Color ToolStripDropDownBackground => Palette.MenuBack;

        public override Color ToolStripBorder => Palette.MenuBorder;

        public override Color MenuBorder => Palette.MenuBorder;

        public override Color MenuItemBorder => Palette.MenuHighlightBack;

        /// <summary>Background of the item under the pointer.</summary>
        public override Color MenuItemSelected => Palette.MenuHighlightBack;

        public override Color MenuItemSelectedGradientBegin => Palette.MenuHighlightBack;

        public override Color MenuItemSelectedGradientEnd => Palette.MenuHighlightBack;

        /// <summary>
        /// Background of a top level item whose drop-down is open, and of the drop-down that is open.
        /// </summary>
        public override Color MenuItemPressedGradientBegin => Palette.MenuHighlightBack;

        public override Color MenuItemPressedGradientMiddle => Palette.MenuHighlightBack;

        public override Color MenuItemPressedGradientEnd => Palette.MenuHighlightBack;

        /// <summary>The gutter left of the icons and check marks.</summary>
        public override Color ImageMarginGradientBegin => Palette.MenuImageMargin;

        public override Color ImageMarginGradientMiddle => Palette.MenuImageMargin;

        public override Color ImageMarginGradientEnd => Palette.MenuImageMargin;

        public override Color SeparatorDark => Palette.MenuBorder;

        public override Color SeparatorLight => Palette.MenuBorder;

        public override Color CheckBackground => Palette.MenuBack;

        public override Color CheckSelectedBackground => Palette.MenuHighlightBack;

        public override Color CheckPressedBackground => Palette.MenuHighlightBack;
    }
}
