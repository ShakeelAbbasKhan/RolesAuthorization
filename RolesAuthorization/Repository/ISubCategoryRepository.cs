using RolesAuthorization.Model;

namespace RolesAuthorization.Repository
{
    public interface ISubCategoryRepository
    {
        Task<List<SubCategory>> GetSubCategoriesAsync();
        Task<SubCategory> GetSubCategoryAsync(int id);
        Task<SubCategory> AddSubCategoryAsync(SubCategory subCategory);
        Task<SubCategory> UpdateSubCategoryAsync(int subCategoryId,SubCategory subCategory);
        Task<SubCategory> DeleteSubCategoryAsync(int id);
    }
}
