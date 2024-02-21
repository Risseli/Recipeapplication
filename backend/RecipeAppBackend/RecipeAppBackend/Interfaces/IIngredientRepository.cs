using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IIngredientRepository
    {
        ICollection<Ingredient> GetIngredients();
        Ingredient GetIngredient(int id);
        bool IngredientExists(int id);
        bool CreateIngredient(Ingredient ingredient);
        bool UpdateIngredient(Ingredient ingredient);
        bool DeleteIngredient(Ingredient ingredinet);
        bool Save();
    }
}
