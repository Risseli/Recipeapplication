using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppBackend.Tests.Repositories
{
    public class RecipeRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Recipes.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Recipes.Add(new Recipe()
                    {
                        Name = "Name",
                        Instructions = "Instructions",
                        Visibility = false,
                        User = new User()
                        {
                            Admin = true,
                            Username = "Username",
                            Password = "Password",
                            Name = "Name",
                            Email = i.ToString() + "email@example.com",
                        },
                        Reviews = new List<Review>()
                        {
                            new Review()
                            {
                                Rating = 1,
                                Comment = "Comment",
                                User = new User()
                                {
                                    Admin = true,
                                    Username = "Username",
                                    Password = "Password",
                                    Name = "Name",
                                    Email = (10 + i).ToString() + "email@example.com",
                                }
                            }
                        },
                        Favorites = new List<Favorite>(),
                        RecipeKeywords = new List<RecipeKeyword>()
                        {
                            new RecipeKeyword()
                            {
                                KeywordId = i,
                                Keyword = new Keyword()
                                {
                                    Word = i.ToString()
                                }
                            }
                        }
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async void RecipeRepository_AddKeyword_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);
            var recipeKeyword = new RecipeKeyword
            {
                RecipeId = 1,
                KeywordId = 1
            };

            // Act
            var result = repository.AddKeyword(recipeKeyword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RecipeRepository_CreateRecipe_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var recipeRepository = new RecipeRepository(dbContext);

            // Create an example recipe
            var recipe = new Recipe()
            {
                Name = "Test Recipe",
                Instructions = "Test Instructions",
                Visibility = true,
                User = new User()
                {
                    Admin = false,
                    Username = "RecipeOwner",
                    Password = "Password",
                    Name = "Recipe Owner",
                    Email = "recipeowner@example.com",
                },
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>(),
                RecipeKeywords = new List<RecipeKeyword>()
        {
            new RecipeKeyword()
            {
                KeywordId = 1, // Assuming keyword ID 1 exists
                Keyword = new Keyword()
                {
                    Word = "TestKeyword"
                }
            }
        }
            };

            var recipeKeywords = new List<RecipeKeyword>();

            // Act
            var result = recipeRepository.CreateRecipe(recipe, recipeKeywords);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void RecipeRepository_DeleteRecipe_ShouldReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);
            var recipe = new Recipe
            {
                Id = 100,
                Name = "Test Recipe",
                Instructions = "Test instructions",
                Visibility = true,
                User = new User
                {
                    Id = 100,
                    Admin = false,
                    Username = "testuser",
                    Password = "testpassword",
                    Name = "Test User",
                    Email = "testuser@example.com"
                }
            };

            // Act
            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();
            var result = repository.DeleteRecipe(recipe);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void RecipeRepository_GetFavoriteCount_Returns0OrGreater()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetFavoriteCount(id);

            // Assert
            result.Should().BeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async void RecipeRepository_GetImagesOfRecipe_ReturnsImages()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetImagesOfRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Image>>();
        }

        [Fact]
        public async void RecipeRepository_GetIngredientsOfRecipe_ReturnsIngredients()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetIngredientsOfRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Ingredient>>();
        }

        [Fact]
        public async void RecipeRepository_GetKeywordsOfRecipe_ReturnsKeywords()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetKeywordsOfRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Keyword>>();
        }

        [Fact]
        public async void RecipeRepository_GetRating_ReturnsDecimalBetweenZeroAndFive()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetRating(id);

            // Assert
            result.Should().BeInRange(0, 5);
        }

        [Fact]
        public async void RecipeRepository_GetRecipe_ReturnsRecipe()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Recipe>();
        }

        [Fact]
        public async void RecipeRepository_GetRecipes_ReturnsRecipes()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetRecipes();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Recipe>>();
        }

        [Fact]
        public async void RecipeRepository_GetRecipeKeywordsOfRecipe_ReturnsRecipeKeywords()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetRecipeKeywordsOfRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<RecipeKeyword>>();
        }

        [Fact]
        public async void RecipeRepository_GetReviewsOfRecipe_ReturnsReviews()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.GetReviewsOfRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Review>>();
        }

        [Fact]
        public async void RecipeRepository_RecipeExists_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.RecipeExists(id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void RecipeRepository_RecipeExists_ReturnsFalse()
        {
            // Arrange
            var id = -1;
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);

            // Act
            var result = repository.RecipeExists(id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async void RecipeRepository_RemoveKeyword_ReturnsTrue()
        {
            //Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);
            var keyword = new RecipeKeyword()
            {
                RecipeId = 1,
                KeywordId = 1
            };

            dbContext.RecipeKeywords.Add(keyword);
            await dbContext.SaveChangesAsync();

            //Act
            var result = repository.RemoveKeyword(keyword);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task RecipeRepository_UpdateRecipe_ShouldReturnTrueAsync()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new RecipeRepository(dbContext);
            var recipe = new Recipe
            {
                Id = 100,
                Name = "Updated Recipe",
                Instructions = "Updated instructions",
                Visibility = true,
                User = new User
                {
                    Id = 100,
                    Admin = false,
                    Username = "testuser",
                    Password = "testpassword",
                    Name = "Test User",
                    Email = "testuser@example.com"
                }
            };

            dbContext.Recipes.Add(recipe);
            await dbContext.SaveChangesAsync();

            // Act
            recipe.Name = "changed name";
            var result = repository.UpdateRecipe(recipe);

            // Assert
            result.Should().BeTrue();
        }
    }
}
