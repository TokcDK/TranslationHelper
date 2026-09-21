using System;
using System.Collections.Generic;
using System.Globalization;

namespace TranslationHelper.Settings
{
    /// <summary>A yes/no setting. The form renders it as a check box.</summary>
    internal abstract class BoolSetting : SettingBase<bool>
    {
        internal override SettingValueKind ValueKind => SettingValueKind.Bool;

        protected override string Format(bool value)
        {
            return value ? bool.TrueString : bool.FalseString;
        }

        protected override bool TryParse(string value, out bool result)
        {
            return bool.TryParse(value, out result);
        }
    }

    /// <summary>
    /// A whole-number setting. The allowed range is declared next to the setting instead of being
    /// hard-coded into the form's event handlers, and a value outside the range falls back to the
    /// default rather than being stored.
    /// </summary>
    internal abstract class IntSetting : SettingBase<int>
    {
        internal override SettingValueKind ValueKind => SettingValueKind.Int;

        internal virtual int Min => int.MinValue;

        internal virtual int Max => int.MaxValue;

        protected override string Format(int value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        protected override bool TryParse(string value, out int result)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out result)
                   && result >= Min
                   && result <= Max;
        }
    }

    /// <summary>A free-form text setting. The form renders it as a text box.</summary>
    internal abstract class StringSetting : SettingBase<string>
    {
        internal override SettingValueKind ValueKind => SettingValueKind.String;

        protected override string Format(string value)
        {
            return value ?? string.Empty;
        }

        protected override bool TryParse(string value, out string result)
        {
            result = value;
            return !string.IsNullOrEmpty(value);
        }
    }

    /// <summary>
    /// A text setting with a list of known-good values. The list is offered by the form but the
    /// stored value stays plain text, so the INI file keeps working when the list changes.
    /// </summary>
    internal abstract class SuggestingStringSetting : StringSetting, ISuggestingSetting
    {
        internal abstract IReadOnlyList<string> Suggestions { get; }

        /// <summary>
        /// Explicit: a member of an interface is public, and this assembly keeps its own surface
        /// internal. Callers reach the list through <see cref="ISuggestingSetting"/>.
        /// </summary>
        IReadOnlyList<string> ISuggestingSetting.Suggestions => Suggestions;
    }

    /// <summary>
    /// A setting limited to a fixed list of values. The form renders it as a drop-down list, and a
    /// value that is not in the list is refused, so the INI file cannot silently hold a format name
    /// that the application does not implement.
    /// </summary>
    internal abstract class ChoiceSetting : SettingBase<string>, IChoiceSetting
    {
        internal override SettingValueKind ValueKind => SettingValueKind.Choice;

        internal abstract IReadOnlyList<string> Choices { get; }

        /// <summary>
        /// Explicit: a member of an interface is public, and this assembly keeps its own surface
        /// internal. Callers reach the list through <see cref="IChoiceSetting"/>.
        /// </summary>
        IReadOnlyList<string> IChoiceSetting.Choices => Choices;

        protected override string Format(string value)
        {
            return value ?? string.Empty;
        }

        protected override bool TryParse(string value, out string result)
        {
            result = value;

            if (string.IsNullOrEmpty(value))
            {
                return false;
            }

            foreach (var choice in Choices)
            {
                if (string.Equals(choice, value, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
