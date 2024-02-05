using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Keyword> Keywords { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<RecipeImage> RecipeImages { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<RecipeKeyword> RecipeKeywords { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RecipeImage>()
                .HasKey(ri => new { ri.RecipeId, ri.ImageId });
            modelBuilder.Entity<RecipeImage>()
                .HasOne(r => r.Recipe)
                .WithMany(ri => ri.RecipeImages)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecipeImage>()
                .HasOne(r => r.Image)
                .WithMany(ri => ri.RecipeImages)
                .HasForeignKey(r => r.ImageId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(ri => new { ri.RecipeId, ri.IngredientId });
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(r => r.Recipe)
                .WithMany(ri => ri.RecipeIngredients)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(r => r.Ingredient)
                .WithMany(ri => ri.RecipeIngredients)
                .HasForeignKey(r => r.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecipeKeyword>()
                .HasKey(ri => new { ri.RecipeId, ri.KeywordId });
            modelBuilder.Entity<RecipeKeyword>()
                .HasOne(r => r.Recipe)
                .WithMany(ri => ri.RecipeKeywords)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<RecipeKeyword>()
                .HasOne(r => r.Keyword)
                .WithMany(ri => ri.RecipeKeywords)
                .HasForeignKey(r => r.KeywordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Favorite>()
                .HasKey(ri => new { ri.RecipeId, ri.UserId });
            modelBuilder.Entity<Favorite>()
                .HasOne(r => r.Recipe)
                .WithMany(ri => ri.Favorites)
                .HasForeignKey(r => r.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Favorite>()
                .HasOne(r => r.User)
                .WithMany(ri => ri.Favorites)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
