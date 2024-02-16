using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Repositories
{
    public class RecipeRepository : IRecipeRepository
    {
        private readonly DataContext _context;
        public RecipeRepository(DataContext dataContext)
        {
            _context = dataContext;
        }

        public bool CreateRecipe(Recipe recipe, List<RecipeKeyword> recipeKeywords)
        {
            _context.RecipeKeywords.AddRange(recipeKeywords);
            _context.Recipes.Add(recipe);
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

        public ICollection<Recipe> GetRecipes()
        {
            return _context.Recipes.Include(r => r.User).OrderBy(r => r.Id).ToList();
        }

        public ICollection<Review> GetReviewsOfRecipe(int id)
        {
            return _context.Reviews.Where(r => r.Recipe.Id == id).Include(r => r.User).ToList();
        }

        public bool RecipeExists(int id)
        {
            return _context.Recipes.Any(r => r.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }
    }
}
