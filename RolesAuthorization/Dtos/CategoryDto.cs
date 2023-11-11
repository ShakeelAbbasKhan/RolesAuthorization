using RolesAuthorization.Model;

namespace RolesAuthorization.Dtos
{
    public class CategoryDto
    {
        public string Name { get; set; }
        public List<SubCategory> SubCategory { get; set; }
    }
}
