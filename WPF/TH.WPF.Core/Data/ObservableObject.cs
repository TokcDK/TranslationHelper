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
    }
}
