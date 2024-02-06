﻿using RecipeAppBackend.Data;
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
    }
}
