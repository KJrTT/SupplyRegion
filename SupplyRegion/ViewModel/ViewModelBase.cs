using System;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SupplyRegion.ViewModel
{
    public abstract class ViewModelBase : ObservableObject, INotifyPropertyChanged
    {
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _errorMessage = string.Empty;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public event EventHandler<string>? ErrorOccurred;

        protected void OnErrorOccurred(string errorMessage)
        {
            ErrorOccurred?.Invoke(this, errorMessage);
        }
    }
}
