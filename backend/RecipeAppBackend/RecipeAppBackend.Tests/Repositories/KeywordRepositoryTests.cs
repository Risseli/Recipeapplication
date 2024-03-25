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
    public class KeywordRepositoryTests
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new DataContext(options);
            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Keywords.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Keywords.Add(new Keyword()
                    {
                        Word = "word" + i.ToString()
                    });
                }
                await databaseContext.SaveChangesAsync();
            }
            return databaseContext;
        }

        [Fact]
        public async Task KeywordRepository_CreateKeyword_ReturnsTrue()
        {
            // Arrange
            var keyword = new Keyword()
            {
                Word = "yo"
            };
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.CreateKeyword(keyword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task KeywordRepository_DeleteKeyword_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Create an example keyword to delete
            var createKeyword = new Keyword()
            {
                Word = "ExampleKeyword"
            };

            // Add the createKeyword entity to the context
            dbContext.Keywords.Add(createKeyword);
            await dbContext.SaveChangesAsync(); // Save changes to database

            // Act
            var result = keywordRepository.DeleteKeyword(createKeyword);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task KeywordRepository_GetKeyword_ReturnsKeyword()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.GetKeyword(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<Keyword>();
        }

        [Fact]
        public async Task KeywordRepository_GetKeywords_ReturnsKeywords()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.GetKeywords();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Keyword>>();
        }

        [Fact]
        public async Task KeywordRepository_GetKeywordsOfRecipe_ReturnsKeywords()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.GetKeywordsOfRecipe(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<Keyword>>();
        }

        [Fact]
        public async Task KeywordRepository_GetRecipeKeywords_ReturnsRecipeKeywords()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.GetRecipeKeywords(id);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<List<RecipeKeyword>>();
        }

        [Fact]
        public async Task KeywordRepository_KeywordExists_ReturnsTrue()
        {
            // Arrange
            var id = 1;
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.KeywordExists(id);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task KeywordRepository_KeywordExists_ReturnsFalse()
        {
            // Arrange
            var id = -1;
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Act
            var result = keywordRepository.KeywordExists(id);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task KeywordRepository_UpdateKeyword_ReturnsTrue()
        {
            // Arrange
            var dbContext = await GetDatabaseContext();
            var keywordRepository = new KeywordRepository(dbContext);

            // Create an example keyword
            var createKeyword = new Keyword()
            {
                Word = "ExampleKeyword"
            };

            // Add the createKeyword entity to the context
            dbContext.Keywords.Add(createKeyword);
            await dbContext.SaveChangesAsync(); // Save changes to database

            // Update the keyword's word
            createKeyword.Word = "UpdatedKeyword";

            // Act
            var result = keywordRepository.UpdateKeyword(createKeyword);

            // Assert
            result.Should().BeTrue();
        }
    }
}
