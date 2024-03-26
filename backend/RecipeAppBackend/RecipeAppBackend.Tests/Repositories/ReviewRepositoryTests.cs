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
    public class ReviewRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Reviews.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Reviews.Add(new Review()
                    {
                        Rating = 1,
                        Comment = "comment",
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
                        },
                        User = new User()
                        {
                            Admin = true,
                            Username = "Username",
                            Password = "Password",
                            Name = "Name",
                            Email = (100+i).ToString() + "email@example.com"
                        }
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }


        [Fact]
        public async Task ReviewRepository_CreateReview_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            // Create an example review
            var createReview = new Review()
            {
                Rating = 1,
                Comment = "Example Comment",
                Recipe = new Recipe()
                {
                    Name = "Example Recipe",
                    Instructions = "Example Instructions",
                    Visibility = false,
                    User = new User()
                    {
                        Admin = true,
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
                    KeywordId = 1,
                    Keyword = new Keyword()
                    {
                        Word = "Keyword"
                    }
                }
            }
                },
                User = new User()
                {
                    Admin = false,
                    Username = "Reviewer",
                    Password = "Password",
                    Name = "Reviewer Name",
                    Email = "reviewer@example.com"
                }
            };

            // Act
            var result = reviewRepository.CreateReview(createReview);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task ReviewRepository_DeleteReview_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            // Create an example review
            var reviewToDelete = new Review()
            {
                Rating = 1,
                Comment = "Example Comment",
                Recipe = new Recipe()
                {
                    Name = "Example Recipe",
                    Instructions = "Example Instructions",
                    Visibility = false,
                    User = new User()
                    {
                        Admin = true,
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
                    KeywordId = 1,
                    Keyword = new Keyword()
                    {
                        Word = "Keyword"
                    }
                }
            }
                },
                User = new User()
                {
                    Admin = false,
                    Username = "Reviewer",
                    Password = "Password",
                    Name = "Reviewer Name",
                    Email = "reviewer@example.com"
                }
            };

            // Add the review to the context
            dbContext.Reviews.Add(reviewToDelete);
            await dbContext.SaveChangesAsync();

            // Act
            var result = reviewRepository.DeleteReview(reviewToDelete);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ReviewRepository_GetReview_ReturnsReview()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            //Act
            var result = reviewRepository.GetReview(id);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Review>();
        }

        [Fact]
        public async void ReviewRepository_GetReviews_ReturnsReviews()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            //Act
            var result = reviewRepository.GetReviews();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Review>>();
        }

        [Fact]
        public async void ReviewRepository_ReviewExists_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            //Act
            var result = reviewRepository.ReviewExists(id);

            //Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async void ReviewRepository_ReviewExists_ReturnsFalse()
        {
            // Arrange
            var id = -1;
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            //Act
            var result = reviewRepository.ReviewExists(id);

            //Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ReviewRepository_UpdateReview_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var reviewRepository = new ReviewRepository(dbContext);

            // Create an example review
            var originalReview = new Review()
            {
                Rating = 3,
                Comment = "Original Comment",
                Recipe = new Recipe()
                {
                    Name = "Original Recipe",
                    Instructions = "Original Instructions",
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
                },
                User = new User()
                {
                    Admin = false,
                    Username = "Reviewer",
                    Password = "Password",
                    Name = "Reviewer Name",
                    Email = "reviewer@example.com"
                }
            };

            // Add the original review to the context and save changes to the database
            dbContext.Reviews.Add(originalReview);
            await dbContext.SaveChangesAsync();

            // Modify the original review
            originalReview.Rating = 5;
            originalReview.Comment = "Updated Comment";

            // Act
            var result = reviewRepository.UpdateReview(originalReview);

            // Assert
            result.Should().BeTrue();
        }
    }
}
