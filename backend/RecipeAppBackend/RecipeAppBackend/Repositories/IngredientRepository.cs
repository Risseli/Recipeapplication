using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Repositories
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly DataContext _context;

        public IngredientRepository(DataContext context)
        {
            _context = context;
        }

        public Ingredient GetIngredient(int id)
        {
            return _context.Ingredients.FirstOrDefault(i => i.Id == id);
        }

        public ICollection<Ingredient> GetIngredients()
        {
            return _context.Ingredients.OrderBy(i => i.Id).ToList();
        }

        public bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(i => i.Id == id);
        }
    }
}
