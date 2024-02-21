using RecipeAppBackend.Data;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IImageRepository
    {
        ICollection<Image> GetImages();
        Image GetImage(int id);
        bool ImageExists(int id);
        bool CreateImage(Image image);
        bool UpdateImage(Image image);
        bool DeleteImage(Image image);
        bool Save();
    }
}
