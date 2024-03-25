using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppBackend.Tests.Repositories
{
    public class IngredientRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Ingredients.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Ingredients.Add(new Ingredient()
                    {
                        Name = "Name",
                        Amount = 1,
                        Unit = "unit",
                        Recipe = new Recipe()
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
                        }
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task IngredientRepository_CreateIngredient_ReturnsTrue()
        {
            // Arrange
            Ingredient createIngredient = new Ingredient()
            {
                Name = "Name",
                Amount = 1,
                Unit = "unit",
                Recipe = new Recipe()
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
                        Email = 1.ToString() + "email@example.com",
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
                                Email = (10 + 1).ToString() + "email@example.com",
                            }
                        }
                    },
                    Favorites = new List<Favorite>(),
                    RecipeKeywords = new List<RecipeKeyword>()
                    {
                        
                    }
                }
            };

            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Act
            var result = ingredientRepository.CreateIngredient(createIngredient);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IngredientRepository_DeleteIngredient_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Create an example ingredient to delete
            var createIngredient = new Ingredient()
            {
                Name = "Name",
                Amount = 1,
                Unit = "unit",
                Recipe = new Recipe()
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
                        Email = "1email@example.com",
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
                        Email = "11email@example.com",
                    }
                }
            },
                    Favorites = new List<Favorite>(),
                    RecipeKeywords = new List<RecipeKeyword>()
                }
            };

            // Add the createIngredient entity to the context
            dbContext.Ingredients.Add(createIngredient);
            await dbContext.SaveChangesAsync(); // Save changes to database

            // Act
            var result = ingredientRepository.DeleteIngredient(createIngredient);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IngredientRepository_GetIngredient_ReturnsIngredient()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Act
            var result = ingredientRepository.GetIngredient(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Ingredient>();
        }

        [Fact]
        public async Task IngredientRepository_GetIngredients_ReturnsIngredients()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Act
            var result = ingredientRepository.GetIngredients();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Ingredient>>();
        }

        [Fact]
        public async Task IngredientRepository_IngredientExists_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Act
            var result = ingredientRepository.IngredientExists(id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IngredientRepository_IngredientExists_ReturnsFalse()
        {
            // Arrange
            var id = -1;
            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Act
            var result = ingredientRepository.IngredientExists(id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task IngredientRepository_UpdateIngredient_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var ingredientRepository = new IngredientRepository(dbContext);

            // Create an example ingredient to update
            var createIngredient = new Ingredient()
            {
                Name = "OriginalName",
                Amount = 1,
                Unit = "originalUnit",
                Recipe = new Recipe()
                {
                    Name = "RecipeName",
                    Instructions = "Instructions",
                    Visibility = false,
                    User = new User()
                    {
                        Admin = true,
                        Username = "Username",
                        Password = "Password",
                        Name = "Name",
                        Email = "1email@example.com",
                    },
                    Reviews = new List<Review>(),
                    Favorites = new List<Favorite>(),
                    RecipeKeywords = new List<RecipeKeyword>()
                }
            };

            // Add the createIngredient entity to the context
            dbContext.Ingredients.Add(createIngredient);
            await dbContext.SaveChangesAsync(); // Save changes to database

            // Update the ingredient properties
            createIngredient.Name = "UpdatedName";
            createIngredient.Unit = "updatedUnit";

            // Act
            var result = ingredientRepository.UpdateIngredient(createIngredient);

            // Assert
            result.Should().BeTrue();
        }
    }
}
