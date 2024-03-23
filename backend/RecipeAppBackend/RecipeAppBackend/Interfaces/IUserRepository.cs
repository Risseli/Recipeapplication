using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User GetUser(int id);
        User GetUserByEmail(string email);
        User GetUserByUsername(string username);
        ICollection<Recipe> GetUsersRecipes(int id);
        ICollection<Recipe> GetUsersFavorites(int id);
        ICollection<Review> GetUsersReviews(int id);
        bool UserExists(int id);
        bool CreateUser(User user);
        bool UpdateUser(User user);
        bool DeleteUser(User user);
        bool AddFavorite(Favorite favorite);
        bool RemoveFavorite(Favorite favorite);
        bool FavoriteExists(int userId, int recipeId);
        bool Save();
        bool ValidateEmail(string email);
    }
}
