using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IReviewRepository
    {
        ICollection<Review> GetReviews();
        Review GetReview(int id);
        bool ReviewExists(int id);
        bool CreateReview(Review review);
        bool Save();
    }
}
