namespace RecipeAppBackend.Models
{
    public class Keyword
    {
        public int Id { get; set; }
        public string Word { get; set; }
        public ICollection<RecipeKeyword> RecipeKeywords { get; set; }
    }
}
