using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Remotely.Shared.ViewModels
{
    public interface IInvokePropertyChanged
    {
        void InvokePropertyChanged(string propertyName = "");
    }

    public class ViewModelBase : INotifyPropertyChanged, IInvokePropertyChanged
    {
        private readonly Dictionary<string, object> _propertyBackingDictionary = new();

        public event PropertyChangedEventHandler PropertyChanged;

        public void InvokePropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected T Get<T>([CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (_propertyBackingDictionary.TryGetValue(propertyName, out var value))
            {
                return (T)value;
            }

            return default(T);
        }

        protected bool Set<T>(T newValue, [CallerMemberName] string propertyName = null)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (EqualityComparer<T>.Default.Equals(newValue, Get<T>(propertyName)))
            {
                return false;
            }

            _propertyBackingDictionary[propertyName] = newValue;
            OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
