using RecipeAppBackend.Models;

namespace RecipeAppBackend.Dto
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        //public User User { get; set; }
        //public Recipe Recipe { get; set; }
    }
}
