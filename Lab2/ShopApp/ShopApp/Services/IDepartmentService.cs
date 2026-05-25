using ShopApp.Models;

namespace ShopApp.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task CreateAsync(Department dept);
        Task UpdateAsync(Department dept);
        Task DeleteAsync(int id);
    }
}
