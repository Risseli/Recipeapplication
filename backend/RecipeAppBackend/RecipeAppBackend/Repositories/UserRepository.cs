using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public User GetUser(int id)
        {
            return _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(u => u.Id).ToList();
        }

        public ICollection<Recipe> GetUsersFavorites(int id)
        {
            return _context.Favorites.Where(f => f.UserId == id).Select(f => f.Recipe).ToList();
        }

        public ICollection<Recipe> GetUsersRecipes(int id)
        {
            return _context.Recipes.Where(r => r.User.Id == id).ToList();
        }

        public ICollection<Review> GetUsersReviews(int id)
        {
            return _context.Reviews.Where(r => r.User.Id == id).ToList();
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }
    }
}
