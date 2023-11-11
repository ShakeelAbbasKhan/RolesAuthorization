using AutoMapper;
using Microsoft.AspNetCore.Http;
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
    public class SubCategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly IMapper _mapper;

        public SubCategoryController(ApplicationDbContext context, ISubCategoryRepository subCategoryRepository, IMapper mapper)
        {
            _context = context;
            _subCategoryRepository = subCategoryRepository;
            _mapper = mapper;
        }

        // GET: api/subcategories
        [HttpGet]
        public async Task<IActionResult> GetSubCategories()
        {
            var std = await _subCategoryRepository.GetSubCategoriesAsync();

            return Ok(_mapper.Map<List<SubCategoryDto>>(std));
        }

        // GET: api/subcategories/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            var cat = await _subCategoryRepository.GetSubCategoryAsync(id);
            if (cat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<SubCategoryDto>(cat));
        }

        // POST: api/subcategories
        [HttpPost]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryDto createSubCategoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cat = await _subCategoryRepository.AddSubCategoryAsync(_mapper.Map<SubCategory>(createSubCategoryDto));


            return CreatedAtAction("GetSubCategory", new { id = cat.Id }, _mapper.Map<CreateSubCategoryDto>(cat));
        }

        // PUT: api/subcategories/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubCategory([FromRoute] int id, [FromBody] UpdateSubCategoryDto updateSubCategoryDto)
        {
            var updateCatExist = await _subCategoryRepository.GetSubCategoryAsync(id);
            if (updateCatExist != null)
            {
                // update method
                var orignalCat = _mapper.Map<SubCategory>(updateSubCategoryDto);    // orignalStd b/c give to db so it is destination
                var updatedCat = await _subCategoryRepository.UpdateSubCategoryAsync(id, orignalCat);

                if (updatedCat != null)
                {
                    return Ok(_mapper.Map<SubCategoryDto>(updatedCat));     // here studentDto b/c return to user
                }
            }

            return NotFound();
        }

        // DELETE: api/subcategories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var deletedCat = await _subCategoryRepository.DeleteSubCategoryAsync(id);
            if (deletedCat == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<SubCategoryDto>(deletedCat));
        }
    }
}
