using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TranslationHelper.Settings;

namespace TranslationHelper.Theming
{
    /// <summary>
    /// The one owner of the theme in use, and of the themes there are.
    /// <para>
    /// It answers "which theme is current", keeps the list of windows that follow it, and is the only
    /// place that reacts to the setting being changed. A window therefore never reads the setting and
    /// never paints itself: it derives from <see cref="ThemableForm"/>, and everything else is this
    /// class's job. That is what keeps a theme change from having to be threaded through every form by
    /// hand, and what makes the change reach windows that are already open — including the settings
    /// window the user is changing the theme in.
    /// </para>
    /// <para>
    /// Adding a theme means implementing <see cref="ITheme"/> and calling <see cref="RegisterTheme"/>.
    /// Nothing else in the application has to be told: the settings form offers whatever
    /// <see cref="AvailableThemes"/> returns.
    /// </para>
    /// </summary>
    internal sealed class ThemeManager
    {
        /// <summary>Name of the theme that reproduces the system look, and the default.</summary>
        internal const string LightName = "Light";

        internal const string DarkName = "Dark";

        private static readonly Lazy<ThemeManager> Lazy =
            new Lazy<ThemeManager>(() => new ThemeManager());

        /// <summary>
        /// The single manager. Built on first use rather than in a static constructor, because reading
        /// a setting loads every setting, and doing that while this type was still being initialised
        /// would load them before the type is fully built.
        /// </summary>
        internal static ThemeManager Instance => Lazy.Value;

        /// <summary>
        /// The themes on offer, in the order the settings form should offer them. A list rather than a
        /// dictionary so that the order is the order they were registered in.
        /// </summary>
        private readonly List<ITheme> _themes = new List<ITheme>();

        private readonly object _gate = new object();

        /// <summary>
        /// Windows currently following the theme. Kept so that a change can be pushed to windows that
        /// are already on screen rather than only to the next one that opens.
        /// </summary>
        private readonly List<Form> _forms = new List<Form>();

        private bool _watchingSetting;

        /// <summary>
        /// The renderer every menu is painted with. Built once and never replaced: it reads the theme
        /// as it paints, so a change of theme needs nothing but a repaint.
        /// </summary>
        private readonly ToolStripRenderer _menuRenderer = new ThemedToolStripRenderer();

        private bool _menuRendererInstalled;

        private ThemeManager()
        {
            RegisterTheme(new LightTheme());
            RegisterTheme(new DarkTheme());
        }

        /// <summary>
        /// Raised after <see cref="CurrentTheme"/> has changed and every window has been repainted.
        /// <para>
        /// For the parts of the application that are not a window and not a control in a window's
        /// tree — the files list draws itself and holds brushes of its own — and therefore cannot be
        /// reached by walking the control tree.
        /// </para>
        /// </summary>
        internal event EventHandler<ITheme> ThemeChanged;

        /// <summary>Names the settings form offers, in the order it offers them.</summary>
        internal IReadOnlyList<string> AvailableThemes
        {
            get
            {
                var names = new List<string>(_themes.Count);

                foreach (var theme in _themes)
                {
                    names.Add(theme.Name);
                }

                return names;
            }
        }

        /// <summary>
        /// The theme the application has always had, and the one used when the stored value names no
        /// theme. Existing installations have no theme key at all, so this is what they keep seeing.
        /// </summary>
        internal ITheme Default => _themes[0];

        /// <summary>
        /// The theme everything is painted with. Read from the setting on every use rather than cached,
        /// so that the setting remains the one owner of which theme the user chose.
        /// </summary>
        internal ITheme CurrentTheme => For(SettingsRegistry.Get<AppearanceSettings.Theme>().Value);

        /// <summary>
        /// The theme registered under <paramref name="name"/>, or <see cref="Default"/> when the name
        /// is not one of <see cref="AvailableThemes"/>. The setting already refuses an unknown value,
        /// so this is the second line of defence for a name that arrives from anywhere else.
        /// </summary>
        internal ITheme For(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                foreach (var theme in _themes)
                {
                    if (string.Equals(theme.Name, name, StringComparison.Ordinal))
                    {
                        return theme;
                    }
                }
            }

            return Default;
        }

        /// <summary>
        /// Adds a theme, or replaces the one of the same name. This is the whole of what a new theme
        /// needs from the rest of the application.
        /// </summary>
        internal void RegisterTheme(ITheme theme)
        {
            if (theme == null)
            {
                return;
            }

            lock (_gate)
            {
                for (var index = 0; index < _themes.Count; index++)
                {
                    if (string.Equals(_themes[index].Name, theme.Name, StringComparison.Ordinal))
                    {
                        _themes[index] = theme;
                        return;
                    }
                }

                _themes.Add(theme);
            }
        }

        /// <summary>
        /// Chooses a theme by name, which writes the setting and nothing else. The setting raises the
        /// change that repaints every window, so this is the one way to change the theme and it goes
        /// through the same door as the settings form.
        /// </summary>
        internal void SetTheme(string name)
        {
            SettingsRegistry.Get<AppearanceSettings.Theme>().SetValueFromObject(name);
        }

        /// <summary>
        /// Makes a window follow the theme: applies it now, and again after every later change of the
        /// setting. Called by <see cref="ThemableForm"/> when the window is shown.
        /// </summary>
        internal void Register(Form form)
        {
            if (form == null)
            {
                return;
            }

            WatchSetting();

            lock (_gate)
            {
                if (!_forms.Contains(form))
                {
                    _forms.Add(form);
                }
            }

            Apply(form);
        }

        /// <summary>
        /// Stops a window following the theme. Called by <see cref="ThemableForm"/> when the window
        /// closes, which is what keeps a closed window from being repainted or held alive.
        /// </summary>
        internal void Unregister(Form form)
        {
            if (form == null)
            {
                return;
            }

            lock (_gate)
            {
                _forms.Remove(form);
            }
        }

        /// <summary>
        /// Applies the current theme to a window and to everything inside it.
        /// <para>
        /// Safe to call on a window that is already themed, so a caller never has to know whether
        /// this is the first application or a repaint after a change.
        /// </para>
        /// </summary>
        internal void Apply(Form form)
        {
            if (form == null || form.IsDisposed)
            {
                return;
            }

            // A setting is written to the INI file by whichever thread changed it, so a change can
            // arrive on a thread that does not own the window. A window without a handle yet has no
            // owning thread and needs no hop.
            if (form.IsHandleCreated && form.InvokeRequired)
            {
                form.Invoke((Action)(() => Apply(form)));
                return;
            }

            ApplyToTree(form);

            InstallMenuRenderer();

            if (form is ThemableForm themed)
            {
                themed.OnThemeApplied();
            }
        }

        /// <summary>
        /// Paints a control and everything inside it, themes the scroll bars of every window in that
        /// tree, and starts following the controls that are added to it afterwards.
        /// <para>
        /// The scroll bars are a second pass over the same tree rather than a styler in
        /// <see cref="ControlStylerRegistry"/>, because they are not part of what a styler paints:
        /// they belong to the window, not to the area the control draws in.
        /// </para>
        /// </summary>
        private void ApplyToTree(Control root)
        {
            if (root == null || root.IsDisposed)
            {
                return;
            }

            // A control can be added by whichever thread is building it, and a control whose window
            // belongs to another thread has to be themed on that thread. A control without a window
            // yet has no owning thread and needs no hop.
            if (root.IsHandleCreated && root.InvokeRequired)
            {
                root.Invoke((Action)(() => ApplyToTree(root)));
                return;
            }

            var theme = CurrentTheme;

            ControlStylerRegistry.Default.ApplyTheme(root, theme);
            NativeScrollBarTheme.Apply(root, theme);

            FollowControlsAddedLater(root);
        }

        /// <summary>
        /// Makes the theme reach a control that appears after its window was themed.
        /// <para>
        /// A window is themed when it opens, and the parts of it the user has not asked for yet do not
        /// exist then. The search window builds its results grid when a search is run, so that grid
        /// would keep the framework's colours and a white scroll bar until the next change of theme.
        /// Subscribing to every container in the tree is what closes that gap, and it is also why the
        /// theme does not have to be reapplied by hand wherever a control happens to be built.
        /// </para>
        /// </summary>
        private void FollowControlsAddedLater(Control container)
        {
            if (container == null)
            {
                return;
            }

            // Both uses of the same method group make equal delegates, so applying the theme again
            // cannot subscribe a second time.
            container.ControlAdded -= OnControlAdded;
            container.ControlAdded += OnControlAdded;

            foreach (Control child in container.Controls)
            {
                FollowControlsAddedLater(child);
            }
        }

        /// <summary>Themes a control that has just been added, and everything inside it.</summary>
        private void OnControlAdded(object sender, ControlEventArgs e)
        {
            if (e == null || e.Control == null)
            {
                return;
            }

            ApplyToTree(e.Control);
        }

        /// <summary>
        /// Installs the themed menu renderer for the whole application.
        /// <para>
        /// A menu is painted through a renderer rather than from colours of its own, and a menu that
        /// does not name a renderer takes the one set here. That is what reaches the menus a window's
        /// control tree cannot: a context menu is a component rather than a control, and the
        /// application builds its context menus when a project is opened, long after the window was
        /// themed. Installing the renderer once covers both, and covers any menu added later.
        /// </para>
        /// </summary>
        private void InstallMenuRenderer()
        {
            if (_menuRendererInstalled)
            {
                return;
            }

            _menuRendererInstalled = true;

            ToolStripManager.Renderer = _menuRenderer;
        }

        /// <summary>
        /// Subscribes to the setting the first time a window asks for the theme, rather than in the
        /// constructor, so that loading every setting does not happen while this type is being built.
        /// </summary>
        private void WatchSetting()
        {
            lock (_gate)
            {
                if (_watchingSetting)
                {
                    return;
                }

                _watchingSetting = true;
            }

            SettingsRegistry.Get<AppearanceSettings.Theme>().Changed += OnThemeSettingChanged;
        }

        private void OnThemeSettingChanged(object sender, SettingChangedEventArgs e)
        {
            ApplyToOpenForms();

            ThemeChanged?.Invoke(this, CurrentTheme);
        }

        /// <summary>
        /// Repaints every window that is following the theme, and forgets the ones that were disposed
        /// without reporting it. A window normally removes itself when it closes, so the pruning is a
        /// safety net rather than the mechanism.
        /// </summary>
        private void ApplyToOpenForms()
        {
            Form[] forms;

            lock (_gate)
            {
                _forms.RemoveAll(form => form.IsDisposed);
                forms = _forms.ToArray();
            }

            foreach (var form in forms)
            {
                Apply(form);
            }
        }
    }
}
