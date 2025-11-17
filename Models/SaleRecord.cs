using System;

namespace ABCPharmacyApi.Models
{
    public class SaleRecord
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MedicineId { get; set; }
        public string MedicineName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal SalePricePerUnit { get; set; }
        public DateTime SoldAt { get; set; } = DateTime.UtcNow;
    }
}