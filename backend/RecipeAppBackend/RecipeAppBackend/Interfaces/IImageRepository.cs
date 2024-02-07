using RecipeAppBackend.Data;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Interfaces
{
    public interface IImageRepository
    {
        ICollection<Image> GetImages();
        Image GetImage(int id);
        bool ImageExists(int id);
    }
}
