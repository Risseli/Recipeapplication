using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IKeywordRepository
    {
        ICollection<Keyword> GetKeywords();
        Keyword GetKeyword(int id);
        ICollection<Keyword> GetKeywordsOfRecipe(int recipeId);
        ICollection<RecipeKeyword> GetRecipeKeywords(int keywordId);
        bool KeywordExists(int id);
        bool CreateKeyword(Keyword keyword);
        bool UpdateKeyword(Keyword keyword);
        bool DeleteKeyword(Keyword keyword);
        bool Save();
    }
}
