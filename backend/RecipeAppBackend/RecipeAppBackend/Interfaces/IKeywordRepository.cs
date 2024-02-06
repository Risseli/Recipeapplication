using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IKeywordRepository
    {
        ICollection<Keyword> GetKeywords();
        Keyword GetKeyword(int id);
        ICollection<Keyword> GetKeywordsOfRecipe(int recipeId);
        bool KeywordExists(int id);
    }
}
