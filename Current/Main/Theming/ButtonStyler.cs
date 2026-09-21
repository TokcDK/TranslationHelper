using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Paints a button, a check box or a radio button.
    /// <para>
    /// These three are the only family the theme cannot describe with colours alone. The framework
    /// draws them in the visual style, and a control drawn in the visual style ignores every colour set
    /// on it, so a check box would keep a light box on a dark surface. A dark theme therefore takes them
    /// out of the visual style and into the flat one, which draws the box and the tick in the control's
    /// own text colour.
    /// </para>
    /// <para>
    /// The colours are written before the rendering, and the order is not a matter of taste. The
    /// framework decides again whether a control may use the visual style every time a colour of that
    /// control changes, and it decides to stop using it as soon as the control has a colour of its own
    /// that differs from its parent's. Assigning the style first and the colour afterwards therefore
    /// loses the style again on the spot, which is exactly the bug this order fixes.
    /// </para>
    /// <para>
    /// Two details belong to the theme only while the theme is what made the control flat. The hover
    /// and press colours of a flat button are otherwise the framework's light greys, which would flash
    /// a near white block under the pointer; and a control that was already flat was given those
    /// colours on purpose, as part of how it looks. <see cref="ThemedControlState"/> is what tells the
    /// two apart.
    /// </para>
    /// </summary>
    internal sealed class ButtonStyler : IControlStyler
    {
        public bool CanStyle(Control control)
        {
            return control is ButtonBase;
        }

        public void Apply(Control control, ITheme theme)
        {
            var toggle = (ButtonBase)control;
            var state = ThemedControlState.For(control);

            // A button carries the button colours and a check box the surface colours, which is the
            // same distinction the theme makes between the two.
            var background = toggle is Button ? theme.ButtonBack : theme.SurfaceBack;
            var text = toggle is Button ? theme.ButtonText : theme.SurfaceText;

            toggle.BackColor = state.AccentColour("BackColor", theme, background);
            toggle.ForeColor = state.AccentColour("ForeColor", theme, toggle.Enabled ? text : theme.DisabledText);

            // Then the rendering, once the colours have settled. Light puts back the style the control
            // had, so that one that was flat in the designer is flat again and one that the framework
            // drew is drawn by the framework again.
            toggle.UseVisualStyleBackColor = state.Value("UseVisualStyleBackColor", theme, false);
            toggle.FlatStyle = state.Value("FlatStyle", theme, FlatStyle.Flat);

            if (theme.IsDark && !state.ChosenAs("FlatStyle", FlatStyle.Flat))
            {
                // The theme is what made this control flat, so the colours that only a flat control
                // shows are the theme's to choose.
                toggle.FlatAppearance.BorderColor = theme.Border;
                toggle.FlatAppearance.MouseOverBackColor = theme.ButtonHoverBack;
                toggle.FlatAppearance.MouseDownBackColor = theme.ButtonPressedBack;
            }
        }
    }
}
