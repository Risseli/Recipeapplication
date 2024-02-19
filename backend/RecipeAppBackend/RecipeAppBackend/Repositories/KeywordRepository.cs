using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Repositories
{
    public class KeywordRepository : IKeywordRepository
    {
        private readonly DataContext _context;

        public KeywordRepository(DataContext context)
        {
            _context = context;
        }

        public bool CreateKeyword(Keyword keyword)
        {
            _context.Keywords.Add(keyword);
            return Save();
        }

        public Keyword GetKeyword(int id)
        {
            return _context.Keywords.FirstOrDefault(k => k.Id == id);
        }

        public ICollection<Keyword> GetKeywords()
        {
            return _context.Keywords.OrderBy(k => k.Id).ToList();
        }

        public ICollection<Keyword> GetKeywordsOfRecipe(int recipeId)
        {
            return _context.RecipeKeywords.Where(rk => rk.RecipeId == recipeId).Select(rk => rk.Keyword).ToList();  
        }

        public bool KeywordExists(int id)
        {
            return _context.Keywords.Any(k => k.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateKeyword(Keyword keyword)
        {
            _context.Update(keyword);
            return Save();
        }
    }
}
