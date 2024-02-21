using Microsoft.EntityFrameworkCore;
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

        public bool CreateImage(Image image)
        {
            _context.Images.Add(image);
            return Save();
        }

        public bool DeleteImage(Image image)
        {
            _context.Remove(image);
            return Save();
        }

        public Image GetImage(int id)
        {
            return _context.Images.Include(i => i.Recipe).FirstOrDefault(i => i.Id == id);
        }

        public ICollection<Image> GetImages()
        {
            return _context.Images.OrderBy(i => i.Id).Include(i => i.Recipe).ToList();
        }

        public bool ImageExists(int id)
        {
            return _context.Images.Any(i => i.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateImage(Image image)
        {
            _context.Update(image);
            return Save();
        }
    }
}
