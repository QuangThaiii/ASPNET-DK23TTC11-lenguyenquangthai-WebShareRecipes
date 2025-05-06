namespace WebShareRecipes.Models
{
    public class IndexViewModel
    {
        public List<CategoryWithRecipesViewModel> Categories { get; set; }
        public List<Recipe> Recipes { get; set; }
        public bool IsSearchResult { get; set; }
        public string SearchQuery { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}