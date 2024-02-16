using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly DataContext _context;

        public ReviewRepository(DataContext context)
        {
            _context = context;
        }

        public Review GetReview(int id)
        {
            return _context.Reviews.Include(r => r.User).Include(r => r.Recipe).FirstOrDefault(r => r.Id == id);
        }

        public ICollection<Review> GetReviews()
        {
            return _context.Reviews.Include(r => r.User).Include(r => r.Recipe).OrderBy(r => r.Id).ToList();
        }

        public bool ReviewExists(int id)
        {
            return _context.Reviews.Any(r => r.Id == id);
        }
    }
}
