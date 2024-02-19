namespace RecipeAppBackend.Dto
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string? Name { get; set; }
        public float Amount { get; set; }
        public string? Unit { get; set; }
    }
}
