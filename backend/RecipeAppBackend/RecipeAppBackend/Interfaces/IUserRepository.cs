using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User GetUser(int id);
        ICollection<Recipe> GetUsersRecipes(int id);
        ICollection<Recipe> GetUsersFavorites(int id);
        ICollection<Review> GetUsersReviews(int id);
        bool UserExists(int id);
        bool CreateUser(User user);
        bool AddFavorite(Favorite favorite);
        bool FavoriteExists(int userId, int recipeId);
        bool Save();
    }
}
