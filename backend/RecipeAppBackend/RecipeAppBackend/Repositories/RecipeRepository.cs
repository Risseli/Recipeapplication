using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace RecipeAppBackend.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly DataContext _context;
        public RecipeRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public bool AddKeyword(RecipeKeyword recipeKeyword)
        {
            _context.RecipeKeywords.Add(recipeKeyword);
            return Save();
        }

        public bool CreateRecipe(Recipe recipe, List<RecipeKeyword> recipeKeywords)
        {
            _context.RecipeKeywords.AddRange(recipeKeywords);
            _context.Recipes.Add(recipe);
            return Save();
        }

        public bool DeleteRecipe(Recipe recipe)
        {
            var ingredients = GetIngredientsOfRecipe(recipe.Id);
            var images = GetImagesOfRecipe(recipe.Id);
            var reviews = GetReviewsOfRecipe(recipe.Id);
            var recipeKeywords = GetRecipeKeywordsOfRecipe(recipe.Id);
            var favorites = _context.Favorites.Where(f => f.RecipeId == recipe.Id);

            _context.RemoveRange(ingredients);
            _context.RemoveRange(images);
            _context.RemoveRange(reviews);
            _context.RemoveRange(recipeKeywords);
            _context.RemoveRange(favorites);

            _context.Remove(recipe);

            return Save();
        }

        public int GetFavoriteCount(int id)
        {
            var count = _context.Reviews.Where(r => r.Recipe.Id == id).ToList();
            return count.Count();
        }

        public ICollection<Image> GetImagesOfRecipe(int id)
        {
            return _context.Images.Where(i => i.Recipe.Id == id).Include(i => i.Recipe).ToList();
        }

        public ICollection<Ingredient> GetIngredientsOfRecipe(int id)
        {
            return _context.Ingredients.Where(i => i.Recipe.Id == id).Include(i => i.Recipe).ToList();
        }

        public ICollection<Keyword> GetKeywordsOfRecipe(int id)
        {
            return _context.RecipeKeywords.Where(rk => rk.RecipeId == id).Select(rk => rk.Keyword).ToList();
        }

        public decimal GetRating(int id)
        {
            var reviews = _context.Reviews.Where(r => r.Recipe.Id == id).ToList();

            if (reviews.Count() <= 0)
                return 0;

            return (decimal)reviews.Sum(r => r.Rating) / reviews.Count();
        }

        public Recipe GetRecipe(int id)
        {
            return _context.Recipes.Include(r => r.User).FirstOrDefault(r => r.Id == id);
        }

        public ICollection<RecipeKeyword> GetRecipeKeywordsOfRecipe(int recipeId)
        {
            return _context.RecipeKeywords.Where(rk => rk.RecipeId == recipeId).Include(rk => rk.Keyword).ToList();
        }

        public ICollection<Recipe> GetRecipes()
        {
            return _context.Recipes.Include(r => r.User).OrderBy(r => r.Id).ToList();
        }

        public ICollection<Review> GetReviewsOfRecipe(int id)
        {
            return _context.Reviews.Where(r => r.Recipe.Id == id).Include(r => r.User).Include(r => r.Recipe).ToList();
        }

        public bool KeywordExists(int recipeId, int keywordId)
        {
            return _context.RecipeKeywords.Any(k => k.RecipeId == recipeId && k.KeywordId == keywordId);
        }

        public bool RecipeExists(int id)
        {
            return _context.Recipes.Any(r => r.Id == id);
        }

        public bool RemoveKeyword(RecipeKeyword recipeKeyword)
        {
            _context.Remove(recipeKeyword);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateRecipe(Recipe recipe)
        {
            _context.Update(recipe);
            return Save();
        }
    }
}
