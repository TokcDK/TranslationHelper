using System;
using System.Collections.Generic;
using System.Globalization;

namespace TranslationHelper.Settings
{
    /// <summary>
    /// A setting with a strongly typed value.
    /// <para>
    /// The value is held here and nowhere else. Before this class existed each setting had two homes
    /// — a static field on <c>AppSettings</c> and a parallel declaration in the settings form — and
    /// the two had already drifted apart. One home means the stored value and the value in use cannot
    /// disagree.
    /// </para>
    /// </summary>
    internal abstract class SettingBase<T> : Setting
    {
        private T _value;
        private bool _hasValue;

        /// <summary>Value used when the INI file has no usable entry for this setting.</summary>
        internal abstract T Default { get; }

        /// <summary>
        /// Current value. Assigning an equal value does nothing, so a control that re-reports the
        /// same value does not cause a pointless write to the INI file.
        /// </summary>
        internal T Value
        {
            get => _hasValue ? _value : Default;
            set
            {
                if (_hasValue && EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }

                _value = value;
                _hasValue = true;
                RaiseChanged();
            }
        }

        internal override string DefaultAsString => Format(Default);

        internal override string ValueAsString => Format(Value);

        internal override object ValueAsObject => Value;

        internal override void SetValueFromString(string value)
        {
            Value = TryParse(value, out var parsed) ? parsed : Default;
        }

        /// <summary>Value to the text written into the INI file.</summary>
        protected virtual string Format(T value)
        {
            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Text from the INI file to a value. Returning false means "not usable", which makes the
        /// caller fall back to the default rather than store nonsense.
        /// </summary>
        protected abstract bool TryParse(string value, out T result);
    }
}
