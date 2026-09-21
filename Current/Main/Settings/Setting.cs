using System;
using System.Collections.Generic;
using System.Globalization;

namespace TranslationHelper.Settings
{
    /// <summary>
    /// Which kind of value a setting holds.
    /// <para>
    /// The settings form uses this to choose an editor for the row. Nothing else in the application
    /// depends on it, so teaching the form a new kind of editor is the only change a new kind needs.
    /// </para>
    /// </summary>
    internal enum SettingValueKind
    {
        Bool,
        Int,
        String,
        Choice,
    }

    /// <summary>
    /// A setting that only accepts one of a fixed list of values.
    /// </summary>
    internal interface IChoiceSetting
    {
        IReadOnlyList<string> Choices { get; }
    }

    /// <summary>
    /// A free-form setting that can suggest known values without restricting what may be entered.
    /// </summary>
    internal interface ISuggestingSetting
    {
        IReadOnlyList<string> Suggestions { get; }
    }

    /// <summary>
    /// Raised after a setting's value has really changed.
    /// </summary>
    internal sealed class SettingChangedEventArgs : EventArgs
    {
        internal SettingChangedEventArgs(Setting setting)
        {
            Setting = setting;
        }

        internal Setting Setting { get; }
    }

    /// <summary>
    /// Base class of every application setting.
    /// <para>
    /// A setting declares <em>what</em> it is: its section, key, label, default and current value.
    /// It knows nothing about INI files (that is <see cref="SettingsIniStore"/>) and nothing about
    /// controls (that is the settings form). That separation is what lets each feature own its own
    /// settings without dragging the user interface along with them.
    /// </para>
    /// </summary>
    internal abstract class Setting
    {
        /// <summary>
        /// INI section the setting is stored in. Section names are feature names, so the file reads
        /// like a description of the application rather than like a list of controls.
        /// </summary>
        internal abstract string Section { get; }

        /// <summary>INI key. Stable: renaming it loses the value already stored on a user's machine.</summary>
        internal abstract string Key { get; }

        /// <summary>Short text shown next to the editor.</summary>
        internal abstract string Label { get; }

        /// <summary>
        /// Longer explanation. Used as the row's tooltip and as the comment written above the key in
        /// the INI file, so the file explains itself when opened in a text editor.
        /// </summary>
        internal virtual string Description => Label;

        internal abstract SettingValueKind ValueKind { get; }

        /// <summary>Position of this setting inside its section, ascending.</summary>
        internal virtual int Order => 0;

        /// <summary>Position of this setting's section on the settings form, ascending.</summary>
        internal virtual int SectionOrder => 0;

        internal abstract string DefaultAsString { get; }

        internal abstract string ValueAsString { get; }

        /// <summary>
        /// Current value in its natural type, boxed. The read counterpart of
        /// <see cref="SetValueFromObject"/>: the settings form can round-trip a control's value
        /// without knowing which kind of setting it is looking at.
        /// </summary>
        internal abstract object ValueAsObject { get; }

        /// <summary>
        /// Applies a value that came from the INI file or from the form. A value that cannot be used
        /// falls back to <see cref="DefaultAsString"/>.
        /// </summary>
        internal abstract void SetValueFromString(string value);

        /// <summary>
        /// Applies a value that came from a form control, where the natural type is already known
        /// (a check box yields a <see cref="bool"/>, a text box a <see cref="string"/>).
        /// </summary>
        internal void SetValueFromObject(object value)
        {
            SetValueFromString(value == null ? null : Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        internal void ResetToDefault()
        {
            SetValueFromString(DefaultAsString);
        }

        /// <summary>
        /// Raised when the value changed. <see cref="SettingsIniStore"/> listens to write the new
        /// value to the INI file straight away.
        /// </summary>
        internal event EventHandler<SettingChangedEventArgs> Changed;

        /// <summary>Called by derived classes once the value has really changed.</summary>
        protected void RaiseChanged()
        {
            Changed?.Invoke(this, new SettingChangedEventArgs(this));
        }

        public override string ToString()
        {
            return Section + "." + Key + "=" + ValueAsString;
        }
    }
}
