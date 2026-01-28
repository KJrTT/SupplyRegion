namespace SupplyRegion.Model
{
    public class StatusSummary
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalAmount { get; set; }
        public int Percent { get; set; }
    }
}

