using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Repositories
{
    public class ImageRepository : IImageRepository
    {
        private readonly DataContext _context;

        public ImageRepository(DataContext context)
        {
            _context = context;
        }

        public Image GetImage(int id)
        {
            return _context.Images.FirstOrDefault(i => i.Id == id);
        }

        public ICollection<Image> GetImages()
        {
            return _context.Images.OrderBy(i => i.Id).ToList();
        }

        public bool ImageExists(int id)
        {
            return _context.Images.Any(i => i.Id == id);
        }
    }
}
