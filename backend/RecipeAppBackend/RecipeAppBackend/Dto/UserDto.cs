using RecipeAppBackend.Models;

namespace RecipeAppBackend.Dto
{
    public class UserDto
    {
        public int Id { get; set; }
        public bool Admin { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
