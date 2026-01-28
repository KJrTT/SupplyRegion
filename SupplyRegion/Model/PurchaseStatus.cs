using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupplyRegion.Model
{
    public static class PurchaseStatus
    {
        public const string New = "Новая";
        public const string Approved = "Согласована";
        public const string Ordered = "Заказана";
        public const string Received = "Получена";
        public const string Cancelled = "Отменена";

        public static List<string> GetAll()
        {
            return new List<string> { New, Approved, Ordered, Received, Cancelled };
        }
    }
}
