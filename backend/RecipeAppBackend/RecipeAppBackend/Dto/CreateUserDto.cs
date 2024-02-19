namespace RecipeAppBackend.Dto
{
    public class CreateUserDto
    {
        public int Id { get; set; }
        public bool? Admin { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
    }
}
