using ShopApp.Models;

namespace ShopApp.Services
{
    public interface ISaleService
    {
        Task<List<Sale>> GetAllAsync();
        Task<Sale?> GetByIdAsync(int id);
        Task CreateAsync(Sale sale);
        Task UpdateAsync(Sale sale);
        Task DeleteAsync(int id);
        Task<List<Good>> GetGoodsAsync();
    }
}
