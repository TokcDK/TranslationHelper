using System;
using System.Windows.Forms;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// Base class of every window that follows the application theme.
    /// <para>
    /// A window takes part by deriving from this class and by nothing else: the theme is applied when
    /// the window is shown and again after every change of the setting, including while the window is
    /// open. <see cref="Form"/> stays the base underneath, so a window that does not care about the
    /// theme keeps working unchanged.
    /// </para>
    /// <para>
    /// Deliberately not abstract. The WinForms designer designs a derived window by creating an
    /// instance of the window's base class first, so an abstract base stops every derived window from
    /// opening in the designer with "the designer must create an instance of type ... but it cannot
    /// because the type is declared as abstract". The class carries no state and its constructor does
    /// nothing, so there is nothing an instance could do wrong.
    /// </para>
    /// </summary>
    public class ThemableForm : Form
    {
        /// <summary>
        /// Applies the theme once the window is shown.
        /// <para>
        /// After <c>base.OnLoad</c> and not before, deliberately: the main window builds its menus
        /// from its own Load handler, so a palette applied first would walk a control tree that the
        /// menus had not been added to yet and leave them unthemed. Everything a window creates while
        /// loading therefore exists by the time the tree is walked.
        /// </para>
        /// </summary>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ThemeManager.Instance.Register(this);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            ThemeManager.Instance.Unregister(this);

            base.OnFormClosed(e);
        }

        /// <summary>
        /// Called after the theme has been applied to this window and its controls.
        /// <para>
        /// The control tree does not hold everything a window can own — a tooltip is a component, not
        /// a control, and so is never walked — so a window that owns such a thing themes it here.
        /// <see cref="ThemeManager.Instance"/>.<see cref="ThemeManager.CurrentTheme"/> is the palette
        /// that has just been applied.
        /// </para>
        /// </summary>
        protected internal virtual void OnThemeApplied()
        {
        }
    }
}
