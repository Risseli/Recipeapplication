namespace RecipeAppBackend.Models
{
    public class RecipeKeyword
    {
        public int RecipeId { get; set; }
        public int KeywordId { get; set; }
        public Recipe Recipe { get; set; }
        public Keyword Keyword { get; set; }
    }
}
