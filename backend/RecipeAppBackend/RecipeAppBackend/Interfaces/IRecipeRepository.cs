using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IRecipeRepository
    {
        ICollection<Recipe> GetRecipes();
        Recipe GetRecipe(int id);
        ICollection<Review> GetReviewsOfRecipe(int id);
        ICollection<Image> GetImagesOfRecipe(int id);
        ICollection<Ingredient> GetIngredientsOfRecipe(int id);
        ICollection<Keyword> GetKeywordsOfRecipe(int id);
        ICollection<RecipeKeyword> GetRecipeKeywordsOfRecipe(int recipeId);
        Decimal GetRating(int id);
        int GetFavoriteCount(int id);
        bool RecipeExists(int id);
        bool AddKeyword(RecipeKeyword recipeKeyword);
        bool RemoveKeyword(RecipeKeyword recipeKeyword);
        bool KeywordExists(int recipeId, int keywordId);
        bool CreateRecipe(Recipe recipe, List<RecipeKeyword> recipeKeywords);
        bool UpdateRecipe(Recipe recipe);
        bool DeleteRecipe(Recipe recipe);
        bool Save(); 
    }
}
