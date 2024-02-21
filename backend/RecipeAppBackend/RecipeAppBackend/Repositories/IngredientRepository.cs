using Microsoft.EntityFrameworkCore;
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

        public bool CreateIngredient(Ingredient ingredient)
        {
            _context.Add(ingredient);
            return Save();
        }

        public bool DeleteIngredient(Ingredient ingredinet)
        {
            _context.Remove(ingredinet);
            return Save();
        }

        public Ingredient GetIngredient(int id)
        {
            return _context.Ingredients.Include(i => i.Recipe).FirstOrDefault(i => i.Id == id);
        }

        public ICollection<Ingredient> GetIngredients()
        {
            return _context.Ingredients.Include(i => i.Recipe).OrderBy(i => i.Id).ToList();
        }

        public bool IngredientExists(int id)
        {
            return _context.Ingredients.Any(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateIngredient(Ingredient ingredient)
        {
            _context.Update(ingredient);
            return Save();
        }
    }
}
