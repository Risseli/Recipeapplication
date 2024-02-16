namespace RecipeAppBackend.Models
{
    public class Image
    {
        public int Id { get; set; }
        public byte[] ImageData { get; set; }
        public Recipe Recipe { get; set; }

        //public ICollection<RecipeImage> RecipeImages { get; set; }
    }
}
