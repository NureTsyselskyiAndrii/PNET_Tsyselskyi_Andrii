using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models;

namespace ShopApp.Services
{
    public class SaleService : ISaleService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<SaleService> _log;

        public SaleService(AppDbContext db, ILogger<SaleService> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<List<Sale>> GetAllAsync()
        {
            _log.LogInformation("Fetching all sales");
            return await _db.Sales.Include(s => s.Good)
                                   .OrderByDescending(s => s.DateSale)
                                   .ToListAsync();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            _log.LogInformation("Fetching sale {Id}", id);
            return await _db.Sales
                .AsNoTracking()
                .Include(s => s.Good)
                .FirstOrDefaultAsync(s => s.SaleId == id);
        }


        public async Task CreateAsync(Sale sale)
        {
            _db.Sales.Add(sale);
            await _db.SaveChangesAsync();
            _log.LogInformation("Created sale CheckNo={CheckNo}", sale.CheckNo);
        }

        public async Task UpdateAsync(Sale sale)
        {
            var existing = await _db.Sales.FindAsync(sale.SaleId);
            if (existing is null) return;

            existing.CheckNo = sale.CheckNo;
            existing.GoodId = sale.GoodId;
            existing.DateSale = sale.DateSale;
            existing.Quantity = sale.Quantity;

            await _db.SaveChangesAsync();
            _log.LogInformation("Updated sale {Id}", sale.SaleId);
        }

        public async Task DeleteAsync(int id)
        {
            var sale = await _db.Sales.FindAsync(id);
            if (sale is null) return;
            _db.Sales.Remove(sale);
            await _db.SaveChangesAsync();
            _log.LogWarning("Deleted sale {Id}", id);
        }

        public Task<List<Good>> GetGoodsAsync() =>
            _db.Goods.OrderBy(g => g.Name).ToListAsync();
    }
}
