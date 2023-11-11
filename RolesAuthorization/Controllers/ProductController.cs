using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RolesAuthorization.Data;
using RolesAuthorization.Dtos;
using RolesAuthorization.Model;
using RolesAuthorization.Repository;

namespace RolesAuthorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public ProductController(ApplicationDbContext context,IMapper mapper,IProductRepository productRepository)
        {
            _context = context;
            _mapper = mapper;
            _productRepository = productRepository;
        }

        // GET: api/products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var std = await _productRepository.GetProductsAsync();

            return Ok(_mapper.Map<List<ProductDto>>(std));
        }

        // GET: api/products/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var cat = await _productRepository.GetProductAsync(id);
            if (cat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(cat));
        }

        // POST: api/products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _productRepository.AddProductAsync(_mapper.Map<Product>(createProductDto));


            return CreatedAtAction("GetProduct", new { id = cat.Id }, _mapper.Map<CreateProductDto>(cat));
        }

        // PUT: api/products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] UpdateProductDto updateProductDto)
        {
            var updateCatExist = await _productRepository.GetProductAsync(id);
            if (updateCatExist != null)
            {
                // update method
                var orignalCat = _mapper.Map<Product>(updateProductDto);    // orignalStd b/c give to db so it is destination
                var updatedCat = await _productRepository.UpdateProductAsync(id, orignalCat);

                if (updatedCat != null)
                {
                    return Ok(_mapper.Map<ProductDto>(updatedCat));     // here studentDto b/c return to user
                }
            }

            return NotFound();
        }

        // DELETE: api/products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var deletedCat = await _productRepository.DeleteProductAsync(id);
            if (deletedCat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<ProductDto>(deletedCat));
        }
    }
}
