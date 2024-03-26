using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
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
    public class UserRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Users.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Users.Add(new User()
                    {
                        Admin = true,
                        Username = "Username",
                        Password = "Password",
                        Name = "Name",
                        Email = i.ToString() + "email@example.com",
                        Recipes = new List<Recipe>() { },
                        Reviews = new List<Review>() { },
                        Favorites = new List<Favorite>() { }
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task UserRepository_AddFavorite_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var user = new User
            {
                Id = 100,
                Admin = true,
                Username = "Username",
                Password = "Password",
                Name = "Name",
                Email = "testuser@example.com",
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };
            var recipe = new Recipe
            {
                Id = 1,
                Name = "Test Recipe",
                Instructions = "Instructions",
                Visibility = true,
                User = user
            };
            var favorite = new Favorite
            {
                UserId = user.Id,
                RecipeId = recipe.Id,
                Recipe = recipe,
                User = user
            };

            // Act
            var result = repository.AddFavorite(favorite);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_CreateUser_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var user = new User
            {
                Admin = true,
                Username = "testuser",
                Password = "testpassword",
                Name = "Test User",
                Email = "testuser@example.com",
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };

            // Act
            var result = repository.CreateUser(user);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_DeleteUser_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var user = new User
            {
                Id = 100,
                Admin = true,
                Username = "testuser",
                Password = "testpassword",
                Name = "Test User",
                Email = "testuser@example.com",
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };

            dbContext.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = repository.DeleteUser(user);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_FavoriteExists_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userId = 1;
            var recipeId = 1;
            var favorite = new Favorite()
            {
                UserId = userId,
                RecipeId = recipeId,
            };
            dbContext.Add(favorite);
            await dbContext.SaveChangesAsync();

            // Act
            var result = repository.FavoriteExists(userId, recipeId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_GetUser_ShouldReturnUser()
        {
            // Arrange
            var userId = 1;
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);

            // Act
            var result = repository.GetUser(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<User>();
        }

        [Fact]
        public async Task UserRepository_GetUsers_ShouldReturnUsers()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);

            // Act
            var result = repository.GetUsers();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<User>>();
        }

        [Fact]
        public async Task UserRepository_GetUserByEmail_ShouldReturnUser()
        {
            // Arrange
            var email = "testuser@example.com";
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var user = new User
            {
                Id = 100,
                Admin = true,
                Username = "testuser",
                Password = "testpassword",
                Name = "Test User",
                Email = email,
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };

            dbContext.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = repository.GetUserByEmail(email);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<User>();
        }

        [Fact]
        public async Task UserRepository_GetUserByUsername_ShouldReturnUser()
        {
            // Arrange
            var username = "testuser";
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var user = new User
            {
                Id = 100,
                Admin = true,
                Username = username,
                Password = "testpassword",
                Name = "Test User",
                Email = "testuser@example.com",
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };

            dbContext.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var result = repository.GetUserByUsername(username);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<User>();
        }

        [Fact]
        public async Task UserRepository_GetUsersFavorites_ShouldReturnRecipes()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userId = 100;
            var user = new User
            {
                Id = userId,
                Admin = true,
                Username = "testuser",
                Password = "testpassword",
                Name = "Test User",
                Email = "email@example.com",
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };

            dbContext.Add(user);
            await dbContext.SaveChangesAsync();

            // Act
            var favorites = repository.GetUsersFavorites(userId);

            // Assert
            favorites.Should().NotBeNull();
            favorites.Should().BeOfType<List<Recipe>>();
        }

        [Fact]
        public async Task UserRepository_GetUsersRecipes_ShouldReturnRecipes()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userId = 1;


            // Act
            var favorites = repository.GetUsersRecipes(userId);

            // Assert
            favorites.Should().NotBeNull();
            favorites.Should().BeOfType<List<Recipe>>();
        }

        [Fact]
        public async Task UserRepository_GetUsersReviews_ShouldReturnReviews()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userId = 1;


            // Act
            var favorites = repository.GetUsersReviews(userId);

            // Assert
            favorites.Should().NotBeNull();
            favorites.Should().BeOfType<List<Review>>();
        }

        [Fact]
        public async Task UserRepository_RemoveFavorite_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var favoriteToRemove = new Favorite()
            {
                UserId = 1,
                RecipeId = 1
            };

            dbContext.Add(favoriteToRemove);
            await dbContext.SaveChangesAsync();

            // Act
            var result = repository.RemoveFavorite(favoriteToRemove);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_UpdateUser_ShouldReturnTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userToUpdate = new User()
            {
                Id = 100,
                Admin = true,
                Username = "testuser",
                Password = "testpassword",
                Name = "Test User",
                Email = "email@example.com",
                Recipes = new List<Recipe>(),
                Reviews = new List<Review>(),
                Favorites = new List<Favorite>()
            };


            dbContext.Add(userToUpdate);
            await dbContext.SaveChangesAsync();

            // Act
            userToUpdate.Username = "changedUsername";
            var result = repository.UpdateUser(userToUpdate);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_UserExists_ShouldReturnTrueWhenUserExists()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userId = 1; // Provide an existing user id in your test database

            // Act
            var result = repository.UserExists(userId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task UserRepository_UserExists_ShouldReturnFalseWhenUserDoesNotExist()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);
            var userId = -1; // Provide a non-existing user id in your test database

            // Act
            var result = repository.UserExists(userId);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user123@test.com")]
        [InlineData("my.email@domain.co.uk")]
        public async void UserRepository_ValidateEmail_ShouldReturnTrueForValidEmails(string email)
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);

            // Act
            var result = repository.ValidateEmail(email);

            // Assert
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("notanemail")]        // Missing @ symbol
        [InlineData("user@domain")]        // Missing top-level domain
        [InlineData("user@domaincom")]     // Missing period before top-level domain
        [InlineData("@domain.com")]        // Missing username
        [InlineData("user@.com")]          // Missing domain
        [InlineData("user@domain.")]       // Missing top-level domain name
        public async void UserRepository_ValidateEmail_ShouldReturnFalseForInvalidEmails(string email)
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var repository = new UserRepository(dbContext);

            // Act
            var result = repository.ValidateEmail(email);

            // Assert
            result.Should().BeFalse();
        }
    }
}
