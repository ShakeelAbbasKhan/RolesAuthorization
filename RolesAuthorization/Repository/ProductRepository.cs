using Microsoft.EntityFrameworkCore;
using RolesAuthorization.Data;
using RolesAuthorization.Model;

namespace RolesAuthorization.Repository
{
   
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<Product> AddProductAsync(Product product)
        {
            await _context.Products.AddAsync(product);

            await _context.SaveChangesAsync();

            return product;
        }

        public async Task<Product> DeleteProductAsync(int id)
        {
            var product = await _context.Products.Include(u => u.Category).FirstOrDefaultAsync(u => u.Id == id);

            if (product != null)
            {
                _context.Remove(product);

                await _context.SaveChangesAsync();
                return product;
            }
            return null;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await _context.Products.Include(c => c.Category).ToListAsync();
            return products;
        }

        public async Task<Product> GetProductAsync(int id)
        {
            var product = await _context.Products.Include(c => c.Category).FirstOrDefaultAsync(u => u.Id == id);
            return product;
        }

        public async Task<Product> UpdateProductAsync(int productId, Product product)
        {
            var existingProduct = await GetProductAsync(productId);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;

                existingProduct.Price = product.Price;
                existingProduct.CategoryId = product.CategoryId;



                await _context.SaveChangesAsync();
                return existingProduct;
            }



            return null;
        }
    }
}
