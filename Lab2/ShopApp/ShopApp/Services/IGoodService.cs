using ShopApp.Models;

namespace ShopApp.Services
{
    public interface IGoodService
    {
        Task<List<Good>> GetAllAsync();
        Task<Good?> GetByIdAsync(int id);
        Task CreateAsync(Good good);
        Task UpdateAsync(Good good);
        Task DeleteAsync(int id);
        Task<List<Department>> GetDepartmentsAsync();
    }
}
