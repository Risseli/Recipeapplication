using System.Reflection.Metadata.Ecma335;

namespace RecipeAppBackend.Models
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Amount { get; set; }
        public string Unit { get; set; }
        public Recipe Recipe { get; set; }

        //public ICollection<RecipeIngredient> RecipeIngredients { get; set; }
    }
}
