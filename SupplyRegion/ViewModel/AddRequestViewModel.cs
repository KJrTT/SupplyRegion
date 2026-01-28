using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupplyRegion.Data;
using SupplyRegion.Model;

namespace SupplyRegion.ViewModel
{
    public partial class AddRequestViewModel : ViewModelBase
    {
        private readonly Repository _repository;
        private Window _window;

        public AddRequestViewModel(Window window)
        {
            _repository = new Repository();
            _window = window;

            Departments = new ObservableCollection<string>
            {
                "Отдел IT",
                "Бухгалтерия",
                "Производство",
                "Отдел снабжения",
                "Администрация",
                "Склад"
            };

            
            SaveCommand = new AsyncRelayCommand(SaveAsync);
            CancelCommand = new RelayCommand(Cancel);

            NewRequest = new PurchaseRequest
            {
                CreatedDate = DateTime.Now,
                Status = PurchaseStatus.New
            };
        }

        [ObservableProperty]
        private PurchaseRequest _newRequest = null!;

        [ObservableProperty]
        private ObservableCollection<string> _departments;

        public IAsyncRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }

        private async Task SaveAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NewRequest.Initiator) ||
                    string.IsNullOrWhiteSpace(NewRequest.Department) ||
                    string.IsNullOrWhiteSpace(NewRequest.ProductName) ||
                    NewRequest.Quantity <= 0 ||
                    NewRequest.EstimatedPrice <= 0)
                {
                    MessageBox.Show("Заполните все обязательные поля и укажите положительные количество и цену.",
                                    "Проверка данных",
                                    MessageBoxButton.OK,
                                    MessageBoxImage.Warning);
                    return;
                }

                IsBusy = true;
                await _repository.AddRequestAsync(NewRequest);
                _window.DialogResult = true;
                _window.Close();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка сохранения: {ex.Message}";
                OnErrorOccurred(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void Cancel()
        {
            _window.DialogResult = false;
            _window.Close();
        }
    }
}
