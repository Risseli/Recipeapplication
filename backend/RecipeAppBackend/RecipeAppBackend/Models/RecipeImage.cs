namespace RecipeAppBackend.Models
{
    public class RecipeImage
    {
        public int RecipeId { get; set; }
        public int ImageId { get; set; }
        public Recipe Recipe { get; set; }
        public Image Image { get; set; }
    }
}
