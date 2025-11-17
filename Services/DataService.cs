using System.Text.Json;
using ABCPharmacyApi.Models;

namespace ABCPharmacyApi.Services
{
    public class DataService
    {
        private readonly string dataDir;
        private readonly string medFile;
        private readonly string salesFile;
        private readonly JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

        private List<Medicine> medicines = new();
        private List<SaleRecord> sales = new();

        public DataService(IWebHostEnvironment env)
        {
            dataDir = Path.Combine(env.ContentRootPath, "data");
            Directory.CreateDirectory(dataDir);
            medFile = Path.Combine(dataDir, "medicines.json");
            salesFile = Path.Combine(dataDir, "sales.json");
            Load();
        }

        private void Load()
        {
            if (File.Exists(medFile))
                medicines = JsonSerializer.Deserialize<List<Medicine>>(File.ReadAllText(medFile)) ?? new();
            if (File.Exists(salesFile))
                sales = JsonSerializer.Deserialize<List<SaleRecord>>(File.ReadAllText(salesFile)) ?? new();
        }

        private void Save()
        {
            File.WriteAllText(medFile, JsonSerializer.Serialize(medicines, jsonOptions));
            File.WriteAllText(salesFile, JsonSerializer.Serialize(sales, jsonOptions));
        }

        public List<Medicine> GetMedicines(string? search = null)
        {
            var q = medicines.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
                q = q.Where(m => m.FullName.Contains(search, StringComparison.OrdinalIgnoreCase));
            return q.OrderBy(m => m.FullName).ToList();
        }

        public Medicine AddMedicine(Medicine m)
        {
            m.Id = Guid.NewGuid();
            m.Price = Math.Round(m.Price, 2);
            medicines.Add(m);
            Save();
            return m;
        }

        public SaleRecord SellMedicine(Guid id, int quantity)
        {
            var med = medicines.FirstOrDefault(x => x.Id == id) ?? throw new Exception("Medicine not found");
            if (quantity <= 0) throw new Exception("Quantity must be > 0");
            if (med.Quantity < quantity) throw new Exception("Insufficient stock");

            med.Quantity -= quantity;
            var sale = new SaleRecord
            {
                Id = Guid.NewGuid(),
                MedicineId = med.Id,
                MedicineName = med.FullName,
                QuantitySold = quantity,
                SalePricePerUnit = med.Price,
                SoldAt = DateTime.UtcNow
            };
            sales.Add(sale);
            Save();
            return sale;
        }

        public List<SaleRecord> GetSales() => sales.OrderByDescending(s => s.SoldAt).ToList();
    }
}