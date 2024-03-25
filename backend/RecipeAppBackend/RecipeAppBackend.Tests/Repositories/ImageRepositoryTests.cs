using FakeItEasy;
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
    public class ImageRepositoryTests
    {

        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Images.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Images.Add(new Image()
                    {
                        ImageData = new byte[] { 72, 101, 108, 108, 111 },
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

        // Update the unit test to use different Id values for the images
        [Fact]
        public async Task ImageRepository_CreateImage_ReturnsTrue()
        {
            // Arrange
            Image createImage = new Image()
            {
                Id = 111, // Update Id value
                ImageData = new byte[] { 72, 101, 108, 108, 111 },
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
                        Email = "1email@example.com", // Update email value
                    },
                    Reviews = new List<Review>(),
                    Favorites = new List<Favorite>(),
                    RecipeKeywords = new List<RecipeKeyword>()
                }
            };

            var dbContext = await GetDatabaseContext();
            var imageRepository = new ImageRepository(dbContext);

            // Act
            var result = imageRepository.CreateImage(createImage);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ImageRepository_DeleteImage_ReturnsTrue()
        {
            // Arrange
            // Create an image to be deleted
            var imageToDelete = new Image()
            {
                Id = 111, // Update Id value
                ImageData = new byte[] { 72, 101, 108, 108, 111 },
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
                        Email = "1email@example.com", // Update email value
                    },
                    Reviews = new List<Review>(),
                    Favorites = new List<Favorite>(),
                    RecipeKeywords = new List<RecipeKeyword>()
                }
            };

            // Create an in-memory database context
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Initialize the database context with some data, including the image to delete
            using (var dbContext = new DataContext(options))
            {
                dbContext.Images.Add(imageToDelete);
                await dbContext.SaveChangesAsync();
            }

            // Act
            using (var dbContext = new DataContext(options))
            {
                // Create the repository instance
                var imageRepository = new ImageRepository(dbContext);

                // Call the delete method
                var result = imageRepository.DeleteImage(imageToDelete);

                // Assert
                result.Should().BeTrue();

                // Check if the image is deleted from the database
                dbContext.Images.FirstOrDefault(i => i.Id == imageToDelete.Id).Should().BeNull();
            }
        }

        [Fact]
        public async Task ImageRepository_GetImage_ReturnsImage()
        {
            // Arrange
            var imageId = 1;
            var dbContext = await GetDatabaseContext();
            var imageRepository = new ImageRepository(dbContext);

            // Act
            var result = imageRepository.GetImage(imageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Image>();
        }

        [Fact]
        public async Task ImageRepository_GetImages_ReturnsImages()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var imageRepository = new ImageRepository(dbContext);

            // Act
            var result = imageRepository.GetImages();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Image>>();
        }

        [Fact]
        public async Task ImageRepository_ImageExists_ReturnsTrue()
        {
            // Arrange
            var imageId = 1;
            var dbContext = await GetDatabaseContext();
            var imageRepository = new ImageRepository(dbContext);

            // Act
            var result = imageRepository.ImageExists(imageId);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ImageRepository_ImageExists_ReturnsFalse()
        {
            // Arrange
            var imageId = -1;
            var dbContext = await GetDatabaseContext();
            var imageRepository = new ImageRepository(dbContext);

            // Act
            var result = imageRepository.ImageExists(imageId);

            // Assert
            result.Should().BeFalse();
        }

    }
}
