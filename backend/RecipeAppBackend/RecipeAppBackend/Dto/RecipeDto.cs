using RecipeAppBackend.Models;

namespace RecipeAppBackend.Dto
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Instructions { get; set; }
        public bool? Visibility { get; set; }
        public int UserId { get; set; }
        public int FavoriteCount { get; set; }
        public decimal Rating { get; set; }
        public ICollection<IngredientDto>? Ingredients { get; set; }
        public ICollection<KeywordDto>? Keywords { get; set; }
        public ICollection<ReviewDto>? Reviews { get; set; }
        public ICollection<ImageDto>? Images { get; set; }
        

        //public User User { get; set; }
        //public ICollection<Review> Reviews { get; set; }
        //public ICollection<Favorite> Favorites { get; set; }
        //public ICollection<RecipeKeyword> RecipeKeywords { get; set; }
        //public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        //public ICollection<RecipeImage> RecipeImages { get; set; }
    }
}
