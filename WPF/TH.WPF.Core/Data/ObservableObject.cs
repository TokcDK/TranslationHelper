using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TH.WPF.Core.Data
{
    /// <summary>
    /// Base class for objects that need to notify property changes
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        /// <summary>
        /// Sets the value of a field and raises PropertyChanged if the value changes
        /// </summary>
        /// <typeparam name="T">Type of the field</typeparam>
        /// <param name="field">Reference to the backing field</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property (automatically set by CallerMemberName)</param>
        /// <returns>True if the value was changed, false otherwise</returns>
        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (Equals(field, value)) return false;
            
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
