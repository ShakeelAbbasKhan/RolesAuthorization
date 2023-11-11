using RolesAuthorization.Model;

namespace RolesAuthorization.Repository
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryAsync(int id);
        Task<Category> AddCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(int categoryId,Category category);
        Task<Category> DeleteCategoryAsync(int id);
    }
}
