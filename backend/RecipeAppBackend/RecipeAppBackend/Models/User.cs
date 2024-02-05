using System.Security.Cryptography;

namespace RecipeAppBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public bool Admin { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<Recipe> Recipes { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<Favorite> Favorites { get; set; }
    }
}
