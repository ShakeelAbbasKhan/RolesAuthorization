using RolesAuthorization.Model;

namespace RolesAuthorization.Dtos
{
    public class CreateSubCategoryDto
    {
        public string Name { get; set; }
        public int CategoryId { get; set; }
    }
}
