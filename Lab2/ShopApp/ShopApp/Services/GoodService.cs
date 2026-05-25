using Microsoft.EntityFrameworkCore;
using ShopApp.Data;
using ShopApp.Models;

namespace ShopApp.Services
{
    public class GoodService : IGoodService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<GoodService> _log;

        public GoodService(AppDbContext db, ILogger<GoodService> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<List<Good>> GetAllAsync()
        {
            _log.LogInformation("Fetching all goods");
            return await _db.Goods.Include(g => g.Department).OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Good?> GetByIdAsync(int id)
        {
            _log.LogInformation("Fetching good {Id}", id);
            return await _db.Goods
                .AsNoTracking()
                .Include(g => g.Department)
                .FirstOrDefaultAsync(g => g.GoodId == id);
        }

        public async Task CreateAsync(Good good)
        {
            _db.Goods.Add(good);
            await _db.SaveChangesAsync();
            _log.LogInformation("Created good {Name}", good.Name);
        }

        public async Task UpdateAsync(Good good)
        {
            var existing = await _db.Goods.FindAsync(good.GoodId);
            if (existing is null) return;

            existing.Name = good.Name;
            existing.Price = good.Price;
            existing.Quantity = good.Quantity;
            existing.Producer = good.Producer;
            existing.DeptId = good.DeptId;
            existing.Description = good.Description;

            await _db.SaveChangesAsync();
            _log.LogInformation("Updated good {Id}", good.GoodId);
        }

        public async Task DeleteAsync(int id)
        {
            var good = await _db.Goods
                .Include(g => g.Sales)
                .FirstOrDefaultAsync(g => g.GoodId == id);

            if (good is null) return;

            _db.Sales.RemoveRange(good.Sales);
            _db.Goods.Remove(good);            

            await _db.SaveChangesAsync();
            _log.LogWarning("Deleted good {Id} with {Count} sales", id, good.Sales.Count);
        }

        public Task<List<Department>> GetDepartmentsAsync() =>
            _db.Departments.OrderBy(d => d.Name).ToListAsync();
    }
}
