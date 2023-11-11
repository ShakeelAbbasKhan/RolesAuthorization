using AutoMapper;
using RolesAuthorization.Dtos;
using RolesAuthorization.Model;

namespace RolesAuthorization.MapperProfile
{
    public class MapperConfig : Profile
    {
        public MapperConfig() 
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryDto, Category>().ReverseMap();

            CreateMap<SubCategory, SubCategoryDto>().ReverseMap();
            CreateMap<CreateSubCategoryDto, SubCategory>().ReverseMap();
            CreateMap<UpdateSubCategoryDto, SubCategory>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<CreateProductDto, Product>().ReverseMap();
            CreateMap<UpdateProductDto, Product>().ReverseMap();

        }
    }
}
