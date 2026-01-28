using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SupplyRegion.Data;
using SupplyRegion.Model;
using SupplyRegion.Services;

namespace SupplyRegion.ViewModel
{
    public partial class MainViewModel : ViewModelBase
    {
        private readonly Repository _repository;
        private readonly CsvExportService _csvService;
        private readonly DialogService _dialogService;

        public MainViewModel()
        {
            _repository = new Repository();
            _csvService = new CsvExportService();
            _dialogService = new DialogService();

            
            Statuses = new ObservableCollection<string> { "Все", "Новая", "Согласована", "Заказана", "Получена", "Отменена" };
            RecentActivities = new ObservableCollection<ActivityItem>();
            Requests = new ObservableCollection<PurchaseRequest>();

            SelectedStatus = "Все";

            
            LoadDataCommand = new AsyncRelayCommand(LoadDataAsync);
            AddRequestCommand = new RelayCommand(AddRequest);
            EditStatusCommand = new AsyncRelayCommand<PurchaseRequest>(EditStatusAsync);
            DeleteRequestCommand = new AsyncRelayCommand<PurchaseRequest>(DeleteRequestAsync);
            ExportToCsvCommand = new AsyncRelayCommand(ExportToCsvAsync);
            ExportSingleToCsvCommand = new AsyncRelayCommand<PurchaseRequest>(ExportSingleToCsvAsync);
            FilterByStatusCommand = new AsyncRelayCommand(FilterByStatusAsync);
            SearchCommand = new AsyncRelayCommand(SearchAsync);

            
            ShowRequestsCommand = new RelayCommand(() => LoadDataCommand.Execute(null));
            ShowAnalyticsCommand = new RelayCommand(ShowAnalytics);
            ShowReportsCommand = new RelayCommand(ShowReports);

            ShowAllRequestsCommand = new RelayCommand(() =>
            {
                SelectedStatus = "Все";
                LoadDataCommand.Execute(null);
            });

            FilterNewCommand = new RelayCommand(() =>
            {
                SelectedStatus = PurchaseStatus.New;
                _ = FilterByStatusCommand.ExecuteAsync(null);
            });

            FilterOrderedCommand = new RelayCommand(() =>
            {
                SelectedStatus = PurchaseStatus.Ordered;
                _ = FilterByStatusCommand.ExecuteAsync(null);
            });

            FilterReceivedCommand = new RelayCommand(() =>
            {
                SelectedStatus = PurchaseStatus.Received;
                _ = FilterByStatusCommand.ExecuteAsync(null);
            });

            
            LoadDataCommand.Execute(null);
        }

        [ObservableProperty]
        private ObservableCollection<PurchaseRequest> _requests;

        [ObservableProperty]
        private ObservableCollection<string> _statuses;

        [ObservableProperty]
        private ObservableCollection<ActivityItem> _recentActivities;

        [ObservableProperty]
        private string _selectedStatus = string.Empty;

        [ObservableProperty]
        private string _searchText = string.Empty;

        [ObservableProperty]
        private int _totalRequests;

        [ObservableProperty]
        private decimal _totalAmount;

        [ObservableProperty]
        private decimal _averagePrice;

        [ObservableProperty]
        private int _completionRate;

        [ObservableProperty]
        private int _newRequestsCount;

        [ObservableProperty]
        private decimal _orderedAmount;

        [ObservableProperty]
        private int _averageQuantity;

        [ObservableProperty]
        private int _activeRequests;

        [ObservableProperty]
        private double _completionRatePercent;

        [ObservableProperty]
        private ObservableCollection<StatusSummary> _statusSummaries = new();

        [ObservableProperty]
        private PurchaseRequest? _selectedRequest;

        partial void OnSearchTextChanged(string value)
        {
            
            _ = SearchCommand.ExecuteAsync(null);
        }

        
        public IAsyncRelayCommand LoadDataCommand { get; }
        public IRelayCommand AddRequestCommand { get; }
        public IAsyncRelayCommand<PurchaseRequest> EditStatusCommand { get; }
        public IAsyncRelayCommand<PurchaseRequest> DeleteRequestCommand { get; }
        public IAsyncRelayCommand ExportToCsvCommand { get; }
        public IAsyncRelayCommand<PurchaseRequest> ExportSingleToCsvCommand { get; }
        public IAsyncRelayCommand FilterByStatusCommand { get; }
        public IAsyncRelayCommand SearchCommand { get; }

        
        public IRelayCommand ShowRequestsCommand { get; }
        public IRelayCommand ShowAnalyticsCommand { get; }
        public IRelayCommand ShowReportsCommand { get; }
        public IRelayCommand ShowAllRequestsCommand { get; }
        public IRelayCommand FilterNewCommand { get; }
        public IRelayCommand FilterOrderedCommand { get; }
        public IRelayCommand FilterReceivedCommand { get; }

        private async Task LoadDataAsync()
        {
            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var requests = await _repository.GetAllRequestsAsync();
                Requests = new ObservableCollection<PurchaseRequest>(requests);
                LoadStatistics();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка загрузки данных: {ex.Message}";
                OnErrorOccurred(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void LoadStatistics()
        {
            if (Requests == null || Requests.Count == 0)
            {
                
                TotalRequests = 0;
                TotalAmount = 0;
                AveragePrice = 0;
                NewRequestsCount = 0;
                OrderedAmount = 0;
                AverageQuantity = 0;
                CompletionRate = 0;
                CompletionRatePercent = 0;
                ActiveRequests = 0;
                return;
            }

            TotalRequests = Requests.Count;
            TotalAmount = Requests.Sum(r => r.Quantity * r.EstimatedPrice);
            AveragePrice = TotalRequests > 0 ? Requests.Average(r => r.EstimatedPrice) : 0;
            NewRequestsCount = Requests.Count(r => r.Status == PurchaseStatus.New);
            OrderedAmount = Requests.Where(r => r.Status == PurchaseStatus.Ordered)
                .Sum(r => r.Quantity * r.EstimatedPrice);
            AverageQuantity = TotalRequests > 0 ? (int)Requests.Average(r => r.Quantity) : 0;

            
            int completedCount = Requests.Count(r => r.Status == PurchaseStatus.Received);
            CompletionRate = TotalRequests > 0 ? (completedCount * 100) / TotalRequests : 0;
            CompletionRatePercent = CompletionRate;

            
            ActiveRequests = Requests.Count(r => r.Status != PurchaseStatus.Cancelled &&
                                                r.Status != PurchaseStatus.Received);

           
            var summaries = Requests
                .GroupBy(r => r.Status)
                .Select(g => new StatusSummary
                {
                    Status = g.Key,
                    Count = g.Count(),
                    TotalAmount = g.Sum(r => r.Quantity * r.EstimatedPrice)
                })
                .ToList();

            foreach (var item in summaries)
            {
                item.Percent = TotalRequests > 0 ? (item.Count * 100 / TotalRequests) : 0;
            }

            StatusSummaries = new ObservableCollection<StatusSummary>(summaries);
        }

        private void LoadRecentActivities()
        {
            RecentActivities.Clear();
        }

        private void AddRequest()
        {
            var addWindow = new View.AddRequestWindow();
            if (_dialogService.ShowDialog(addWindow) == true)
            {
                LoadDataCommand.Execute(null);
            }
        }

        private void ShowAnalytics()
        {
            var window = new View.AnalyticsWindow
            {
                DataContext = this
            };

            _dialogService.ShowDialog(window);
        }

        private void ShowReports()
        {
            _dialogService.ShowMessage(
                "Раздел отчетов пока не реализован полностью. Вы можете выгрузить заявки в CSV и построить отчеты в Excel.",
                "Отчеты");
        }

        private async Task EditStatusAsync(PurchaseRequest? request)
        {
            if (request == null) return;

            var changeStatusWindow = new View.ChangeStatusWindow(request.Status);
            if (_dialogService.ShowDialog(changeStatusWindow) == true)
            {
                var newStatus = changeStatusWindow.SelectedStatus;
                if (!string.IsNullOrWhiteSpace(newStatus) && newStatus != request.Status)
                {
                    try
                    {
                        IsBusy = true;
                        await _repository.UpdateStatusAsync(request.Id, newStatus);

                        
                        request.Status = newStatus;
                        LoadStatistics();

                        RecentActivities.Insert(0, new ActivityItem
                        {
                            Icon = "✏️",
                            Message = $"Статус заявки №{request.Id} изменен на '{newStatus}'",
                            Timestamp = "Только что"
                        });
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage = $"Ошибка изменения статуса: {ex.Message}";
                        OnErrorOccurred(ErrorMessage);
                    }
                    finally
                    {
                        IsBusy = false;
                    }
                }
            }
        }

        private async Task DeleteRequestAsync(PurchaseRequest? request)
        {
            if (request == null) return;

            if (_dialogService.ShowConfirmation(
                $"Удалить заявку №{request.Id} на '{request.ProductName}'?",
                "Подтверждение удаления"))
            {
                try
                {
                    IsBusy = true;
                    await _repository.DeleteRequestAsync(request.Id);
                    Requests.Remove(request);
                    LoadStatistics();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException)
                {
                    
                    await LoadDataAsync();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Ошибка удаления: {ex.Message}";
                    OnErrorOccurred(ErrorMessage);
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private async Task ExportToCsvAsync()
        {
            try
            {
                IsBusy = true;
                string filePath = _dialogService.ShowSaveFileDialog(
                    "CSV файлы|*.csv",
                    $"Заявки_на_закупку_{DateTime.Now:yyyyMMdd}.csv");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await _csvService.ExportToCsvAsync(Requests.ToList(), filePath);

                    RecentActivities.Insert(0, new ActivityItem
                    {
                        Icon = "📥",
                        Message = "Экспорт всех заявок в CSV выполнен",
                        Timestamp = "Только что"
                    });

                    _dialogService.ShowMessage("Экспорт завершен", "Файл успешно сохранен");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка экспорта: {ex.Message}";
                OnErrorOccurred(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ExportSingleToCsvAsync(PurchaseRequest? request)
        {
            if (request == null) return;

            try
            {
                IsBusy = true;
                string filePath = _dialogService.ShowSaveFileDialog(
                    "CSV файлы|*.csv",
                    $"Заявка_{request.Id}_{DateTime.Now:yyyyMMdd}.csv");

                if (!string.IsNullOrEmpty(filePath))
                {
                    await _csvService.ExportToCsvAsync(new List<PurchaseRequest> { request }, filePath);

                    RecentActivities.Insert(0, new ActivityItem
                    {
                        Icon = "📥",
                        Message = $"Экспорт заявки №{request.Id} в CSV выполнен",
                        Timestamp = "Только что"
                    });

                    _dialogService.ShowMessage("Экспорт завершен", "Файл успешно сохранен");
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка экспорта: {ex.Message}";
                OnErrorOccurred(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FilterByStatusAsync()
        {
            try
            {
                IsBusy = true;
                if (SelectedStatus == "Все")
                {
                    await LoadDataAsync();
                }
                else
                {
                    var filtered = await _repository.GetRequestsByStatusAsync(SelectedStatus);
                    Requests = new ObservableCollection<PurchaseRequest>(filtered);
                    LoadStatistics();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка фильтрации: {ex.Message}";
                OnErrorOccurred(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SearchAsync()
        {
            try
            {
                IsBusy = true;
                if (string.IsNullOrWhiteSpace(SearchText))
                {
                    await LoadDataAsync();
                }
                else
                {
                    var results = await _repository.SearchByProductNameAsync(SearchText);
                    Requests = new ObservableCollection<PurchaseRequest>(results);
                    LoadStatistics();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка поиска: {ex.Message}";
                OnErrorOccurred(ErrorMessage);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void Dispose()
        {
            //
        }
    }
}
