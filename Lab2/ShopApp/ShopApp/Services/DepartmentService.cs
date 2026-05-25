using ShopApp.Data;
using ShopApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ShopApp.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly AppDbContext _db;
        private readonly ILogger<DepartmentService> _log;

        public DepartmentService(AppDbContext db, ILogger<DepartmentService> log)
        {
            _db = db;
            _log = log;
        }

        public async Task<List<Department>> GetAllAsync()
        {
            _log.LogInformation("Fetching all departments");
            return await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            _log.LogInformation("Fetching department {Id}", id);
            return await _db.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.DeptId == id);
        }

        public async Task CreateAsync(Department dept)
        {
            _db.Departments.Add(dept);
            await _db.SaveChangesAsync();
            _log.LogInformation("Created department {Name}", dept.Name);
        }

        public async Task UpdateAsync(Department dept)
        {
            var existing = await _db.Departments.FindAsync(dept.DeptId);
            if (existing is null) return;

            existing.Name = dept.Name;
            existing.Info = dept.Info;

            await _db.SaveChangesAsync();
            _log.LogInformation("Updated department {Id}", dept.DeptId);
        }

        public async Task DeleteAsync(int id)
        {
            var dept = await _db.Departments.FindAsync(id);
            if (dept is null) return;
            _db.Departments.Remove(dept);
            await _db.SaveChangesAsync();
            _log.LogWarning("Deleted department {Id}", id);
        }
    }
}
