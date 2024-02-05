using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RecipeAppBackend.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Instructions { get; set; }
        public bool Visibility { get; set; }
        public User User { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
        public ICollection<RecipeKeyword> RecipeKeywords { get; set; }
        public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public ICollection<RecipeImage> RecipeImages { get; set; }
    }
}
