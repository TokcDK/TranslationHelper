using System.Collections.Generic;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// The stylers there are, and the walk that puts them to work.
    /// <para>
    /// It walks the control tree rather than being handed a list of controls, so a control added to a
    /// designer is themed without anyone having to register it. For each control it asks the stylers in
    /// turn and lets the first one that recognises the control paint it.
    /// </para>
    /// <para>
    /// The order the stylers are registered in is therefore part of the design, not an accident: the
    /// shapes overlap — a check box and a button are both <see cref="ButtonBase"/>, a combo box is a
    /// <see cref="ListControl"/> like a list box, a tab page is a <see cref="Panel"/> — and taking the
    /// first match is what keeps a check box from being painted as a button. Register a styler for a
    /// derived control before the styler for its base.
    /// </para>
    /// </summary>
    internal sealed class ControlStylerRegistry
    {
        private static readonly ControlStylerRegistry Shared = CreateDefault();

        /// <summary>The registry the application uses, built once.</summary>
        internal static ControlStylerRegistry Default => Shared;

        private readonly List<IControlStyler> _stylers = new List<IControlStyler>();

        /// <summary>
        /// Adds a styler. Later registrations are only reached by a control that no earlier one
        /// recognised, so this is also how a styler is made to take precedence over another.
        /// </summary>
        internal void Register(IControlStyler styler)
        {
            if (styler != null)
            {
                _stylers.Add(styler);
            }
        }

        /// <summary>Paints <paramref name="root"/> and everything inside it.</summary>
        internal void ApplyTheme(Control root, ITheme theme)
        {
            if (root == null)
            {
                return;
            }

            ApplyToControl(root, theme);

            foreach (Control child in root.Controls)
            {
                ApplyTheme(child, theme);
            }
        }

        private void ApplyToControl(Control control, ITheme theme)
        {
            foreach (var styler in _stylers)
            {
                if (!styler.CanStyle(control))
                {
                    continue;
                }

                styler.Apply(control, theme);
                return;
            }
        }

        /// <summary>
        /// The stylers the application ships with, in the order they must be asked. A control the
        /// application has never heard of is still painted, by <see cref="SurfaceStyler"/>, which is
        /// last and accepts everything: leaving it alone would leave a light patch on a dark window,
        /// which is the one thing the theme exists to prevent.
        /// </summary>
        private static ControlStylerRegistry CreateDefault()
        {
            var registry = new ControlStylerRegistry();

            // Most specific shape first, because the shapes overlap.
            registry.Register(new ToolStripStyler());
            registry.Register(new DataGridViewStyler());
            registry.Register(new TabControlStyler());
            registry.Register(new TabPageStyler());
            registry.Register(new ComboBoxStyler());
            registry.Register(new ListBoxStyler());
            registry.Register(new TextBoxStyler());
            registry.Register(new ButtonStyler());
            registry.Register(new LinkLabelStyler());
            registry.Register(new LabelStyler());
            registry.Register(new SplitContainerStyler());
            registry.Register(new FormStyler());
            registry.Register(new PanelStyler());
            registry.Register(new SurfaceStyler());

            return registry;
        }
    }
}
