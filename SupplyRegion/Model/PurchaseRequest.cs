using System;
using System.ComponentModel;

namespace SupplyRegion.Model
{
    public class PurchaseRequest : INotifyPropertyChanged
    {
        private int _quantity;
        private decimal _estimatedPrice;
        private string _status = PurchaseStatus.New;

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Initiator { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;

        public int Quantity
        {
            get => _quantity;
            set
            {
                if (_quantity != value)
                {
                    _quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public decimal EstimatedPrice
        {
            get => _estimatedPrice;
            set
            {
                if (_estimatedPrice != value)
                {
                    _estimatedPrice = value;
                    OnPropertyChanged(nameof(EstimatedPrice));
                    OnPropertyChanged(nameof(TotalPrice));
                }
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }
        public string? Notes { get; set; }

        public decimal TotalPrice => Quantity * EstimatedPrice;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
