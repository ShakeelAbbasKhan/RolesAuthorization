using Microsoft.EntityFrameworkCore;
using RolesAuthorization.Data;
using RolesAuthorization.Model;

namespace RolesAuthorization.Repository
{
   
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Category> AddCategoryAsync(Category category)
        {
            await _context.Categories.AddAsync(category);

            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<Category> DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories.Include(u => u.SubCategories).FirstOrDefaultAsync(u => u.Id == id);

            if (category != null)
            {
                _context.Remove(category);

                await _context.SaveChangesAsync();
                return category;
            }
            return null;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.Include(c => c.SubCategories).ToListAsync();
            return categories;
        }

        public async Task<Category> GetCategoryAsync(int id)
        {
            var student = await _context.Categories.Include(c => c.SubCategories).FirstOrDefaultAsync(u => u.Id == id);
            return student;
        }

        public async Task<Category> UpdateCategoryAsync(int categoryId, Category category)
        {
            var existingCat = await GetCategoryAsync(categoryId);
            if (existingCat != null)
            {
                existingCat.Name = category.Name;
                await _context.SaveChangesAsync();
                return existingCat;
            }



            return null;
        }
    }
}
