using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TranslationHelper.Models
{
    /// <summary>
    /// Base class for the models the workspace is bound to.
    /// <para>
    /// It exists so a view can follow a model without the model knowing the view: the model only
    /// announces that a property changed, and the binding layer decides what to do about it. Nothing
    /// here references WinForms, which is what lets the models be used — and tested — without a UI.
    /// </para>
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised after a property of this object has been given a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Announces that <paramref name="propertyName"/> changed.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Assigns <paramref name="field"/> and announces the change, or does nothing when the value
        /// is already the one being assigned.
        /// </summary>
        /// <returns>True when the value was changed.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;

            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
