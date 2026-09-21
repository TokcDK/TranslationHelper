using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// What a control looked like before the theme touched it, which is what makes a theme
    /// reversible.
    /// <para>
    /// A dark theme has to paint over colours that were chosen on purpose: a panel left white inside a
    /// dark window would be the one thing a dark theme exists to prevent. Switching back therefore has
    /// to know what it painted over. This class answers two questions about a control — did anyone
    /// choose the value of a property, and what was the value — and it is the only place that knows.
    /// </para>
    /// <para>
    /// "Chosen" is asked with <c>ShouldSerializeValue</c>, which is the very question the WinForms
    /// designer asks when it decides whether a property is worth writing out. It is the only reliable
    /// way to tell a deliberate colour from an inherited one: a label inside a white panel reports
    /// <see cref="Control.BackColor"/> as white without ever having been given a colour, and a theme
    /// that mistook that for a choice would put a grey box of the label's own on the white panel.
    /// </para>
    /// <para>
    /// Both answers are taken once, the first time the theme touches a control, because the theme's own
    /// writes would otherwise make every property look chosen from the second pass onwards.
    /// </para>
    /// </summary>
    internal sealed class ThemedControlState
    {
        /// <summary>
        /// The properties the theme writes, and therefore the properties whose earlier value has to be
        /// known before the first write. A control that has none of them is unaffected.
        /// </summary>
        private static readonly string[] Watched =
        {
            "BackColor",
            "ForeColor",
            "FlatStyle",
            "UseVisualStyleBackColor",
            "BackgroundColor",
            "GridColor",
            "EnableHeadersVisualStyles",
            "LinkColor",
            "ActiveLinkColor",
            "VisitedLinkColor",
            "DrawMode",
        };

        private static readonly ConditionalWeakTable<Control, ThemedControlState> States =
            new ConditionalWeakTable<Control, ThemedControlState>();

        /// <summary>What each property was before the theme arrived.</summary>
        private readonly Dictionary<string, Recorded> _recorded =
            new Dictionary<string, Recorded>(StringComparer.Ordinal);

        /// <summary>One property as it was: its value, and whether anyone had chosen it.</summary>
        private sealed class Recorded
        {
            internal object Value;

            internal bool Chosen;
        }

        private ThemedControlState(Control control)
        {
            var properties = TypeDescriptor.GetProperties(control);

            foreach (var name in Watched)
            {
                var property = properties[name];

                if (property == null)
                {
                    continue;
                }

                _recorded[name] = new Recorded
                {
                    Value = property.GetValue(control),
                    Chosen = property.ShouldSerializeValue(control),
                };
            }
        }

        /// <summary>
        /// The state of <paramref name="control"/>, taken now when this is the first time the theme has
        /// touched it. Held weakly, so a window that has been closed does not keep its controls alive
        /// through this table.
        /// </summary>
        internal static ThemedControlState For(Control control)
        {
            return States.GetValue(control, key => new ThemedControlState(key));
        }

        /// <summary>
        /// The colour to paint a surface, an editor or a caption with.
        /// <para>
        /// Dark paints the palette's colour. Light gives back the colour that was chosen and, where
        /// there was none, <see cref="Color.Empty"/>, which takes the colour off the control so that it
        /// inherits the colour of whatever it sits on again. Clearing rather than assigning the light
        /// palette's colour is the point: assigning would give every control a colour of its own and
        /// lose the difference between a panel that was meant to be white and one that was not.
        /// </para>
        /// </summary>
        internal Color Colour(string property, ITheme theme, Color painted)
        {
            return theme.IsDark ? painted : ChosenColour(property, Color.Empty);
        }

        /// <summary>
        /// The colour to paint a control with that the theme keeps its hands off where a colour was
        /// chosen for it.
        /// <para>
        /// A button given a colour on purpose is an accent — the blue that means search, the green that
        /// means replace — and it keeps that colour in both themes. Those accents read as well on a dark
        /// surface as on a light one, and repainting them would take away the only thing that tells two
        /// buttons apart. Where nothing was chosen the palette paints, as on any surface.
        /// </para>
        /// </summary>
        internal Color AccentColour(string property, ITheme theme, Color painted)
        {
            return ChosenColour(property, theme.IsDark ? painted : Color.Empty);
        }

        /// <summary>
        /// A value the theme writes in both themes, and for which there is no "nothing was chosen" to go
        /// back to: a rendering style, a flag, or a colour that refuses to be cleared. Light puts back
        /// the value the control had.
        /// <para>
        /// Putting back the value rather than a framework default is not a detail. Whether a button may
        /// be drawn in the visual style is not a fixed default at all — the framework decides it again
        /// from the colour of the control's parent every time a colour changes, so a button on a panel
        /// with a colour of its own reports false while the same button on a plain panel reports true.
        /// Restoring "true" would therefore change the look of every button on a coloured panel instead
        /// of restoring it.
        /// </para>
        /// </summary>
        internal T Value<T>(string property, ITheme theme, T painted)
        {
            if (theme.IsDark)
            {
                return painted;
            }

            return _recorded.TryGetValue(property, out var recorded) && recorded.Value is T original
                ? original
                : painted;
        }

        /// <summary>
        /// True when <paramref name="property"/> was chosen and its value is <paramref name="value"/>.
        /// Used to recognise a control that was already drawn the way the theme draws it, and whose
        /// remaining details are therefore part of the design rather than part of the theme.
        /// </summary>
        internal bool ChosenAs<T>(string property, T value)
        {
            return _recorded.TryGetValue(property, out var recorded)
                && recorded.Chosen
                && recorded.Value is T typed
                && Equals(typed, value);
        }

        private Color ChosenColour(string property, Color whenNothingChosen)
        {
            return _recorded.TryGetValue(property, out var recorded)
                && recorded.Chosen
                && recorded.Value is Color chosen
                ? chosen
                : whenNothingChosen;
        }
    }
}
