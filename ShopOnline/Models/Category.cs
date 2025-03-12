namespace ShopOnline.Models
{
    public class Category
    {
        public string Name { get; set; }
        public List<SubCategory> SubCategories { get; set; }
    }
}
