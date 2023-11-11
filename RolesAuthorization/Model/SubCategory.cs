namespace RolesAuthorization.Model
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Foreign Key for Category
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
