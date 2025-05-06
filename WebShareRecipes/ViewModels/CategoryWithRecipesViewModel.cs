namespace WebShareRecipes.Models
{
    public class CategoryWithRecipesViewModel
    {
        public Category Category { get; set; }
        public List<Recipe> Recipes { get; set; }
    }
}