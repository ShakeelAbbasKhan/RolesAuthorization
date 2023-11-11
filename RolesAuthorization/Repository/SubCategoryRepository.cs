using Microsoft.EntityFrameworkCore;
using RolesAuthorization.Data;
using RolesAuthorization.Model;

namespace RolesAuthorization.Repository
{
   
    public class SubCategoryRepository : ISubCategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public SubCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SubCategory> AddSubCategoryAsync(SubCategory subCategory)
        {
            await _context.SubCategories.AddAsync(subCategory);

            await _context.SaveChangesAsync();

            return subCategory;
        }

        public async Task<SubCategory> DeleteSubCategoryAsync(int id)
        {
            var subCategory = await _context.SubCategories.Include(u => u.Category).FirstOrDefaultAsync(u => u.Id == id);

            if (subCategory != null)
            {
                _context.Remove(subCategory);

                await _context.SaveChangesAsync();
                return subCategory;
            }
            return null;
        }

        public async Task<List<SubCategory>> GetSubCategoriesAsync()
        {
                var subCategories = await _context.SubCategories.Include(c => c.Category).ToListAsync();
                return subCategories;
        }

        public async Task<SubCategory> GetSubCategoryAsync(int id)
        {
                var subCategory = await _context.SubCategories.Include(c => c.Category).FirstOrDefaultAsync(u => u.Id == id);
                return subCategory;
        }

        public async Task<SubCategory> UpdateSubCategoryAsync(int subCategoryId, SubCategory subCategory)
        {
            var existingSubCat = await GetSubCategoryAsync(subCategoryId);
            if (existingSubCat != null)
            {
                existingSubCat.Name = subCategory.Name;
                existingSubCat.CategoryId = subCategory.CategoryId;
                await _context.SaveChangesAsync();
                return existingSubCat;
            }



            return null;
        }
    }
}
