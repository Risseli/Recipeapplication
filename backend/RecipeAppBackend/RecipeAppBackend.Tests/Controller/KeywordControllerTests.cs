using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using RecipeAppBackend.Controllers;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppBackend.Tests.Controller
{
    public class KeywordControllerTests
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly DefaultHttpContext _httpContext;

        public KeywordControllerTests()
        {
            _keywordRepository = A.Fake<IKeywordRepository>();
            _authService = A.Fake<IAuthService>();
            _mapper = A.Fake<IMapper>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void KeywordController_GetKeywords_ReturnsOk_WhenKeywordsExist()
        {
            // Arrange
            var keywords = new List<Keyword> { new Keyword { Id = 1, Word = "TestKeyword" } };
            var keywordDtos = new List<KeywordDto> { new KeywordDto { Id = 1, Word = "TestKeyword" } };

            A.CallTo(() => _keywordRepository.GetKeywords()).Returns(keywords);
            A.CallTo(() => _mapper.Map<List<KeywordDto>>(keywords)).Returns(keywordDtos);

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetKeywords();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(keywordDtos);
        }

        [Fact]
        public void KeywordController_GetKeywords_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new KeywordController(_keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.GetKeywords();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void KeywordController_GetKeyword_ReturnsOk_WhenKeywordExists()
        {
            // Arrange
            int keywordId = 1; // Assuming this keyword ID exists
            var keyword = new Keyword { Id = keywordId, Word = "TestKeyword" };
            var keywordDto = new KeywordDto { Id = keywordId, Word = "TestKeyword" };

            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(true);
            A.CallTo(() => _keywordRepository.GetKeyword(keywordId)).Returns(keyword);
            A.CallTo(() => _mapper.Map<KeywordDto>(keyword)).Returns(keywordDto);

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(keywordDto);
        }

        [Fact]
        public void KeywordController_GetKeyword_ReturnsNotFound_WhenKeywordDoesNotExist()
        {
            // Arrange
            int keywordId = 1; // Assuming this keyword ID doesn't exist

            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(false);

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void KeywordController_GetKeyword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int keywordId = 1; // Assuming this keyword ID exists

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");

            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(true);

            // Act
            var result = controller.GetKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void KeywordController_GetKeywordsOfRecipe_ReturnsOk_WhenKeywordsExistForRecipe()
        {
            // Arrange
            int recipeId = 1; // Assuming this recipe ID exists
            var keywords = new List<Keyword> { new Keyword { Id = 1, Word = "TestKeyword" } };
            var keywordDtos = new List<KeywordDto> { new KeywordDto { Id = 1, Word = "TestKeyword" } };

            A.CallTo(() => _keywordRepository.GetKeywordsOfRecipe(recipeId)).Returns(keywords);
            A.CallTo(() => _mapper.Map<List<KeywordDto>>(keywords)).Returns(keywordDtos);

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetKeywordsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(keywordDtos);
        }

        [Fact]
        public void KeywordController_GetKeywordsOfRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int recipeId = 1; // Assuming this recipe ID exists

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.GetKeywordsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void KeywordController_CreateKeyword_ReturnsOk_WhenKeywordCreationIsSuccessful()
        {
            // Arrange
            var createKeyword = new KeywordDto { Id = 1, Word = "TestKeyword" };
            var keyword = new Keyword { Id = 1, Word = "TestKeyword" }; // Assuming keyword doesn't exist

            A.CallTo(() => _keywordRepository.GetKeywords()).Returns(new List<Keyword>()); // Simulate that no existing keywords
            A.CallTo(() => _mapper.Map<Keyword>(createKeyword)).Returns(keyword);
            A.CallTo(() => _keywordRepository.CreateKeyword(keyword)).Returns(true);

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.CreateKeyword(createKeyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void KeywordController_CreateKeyword_ReturnsBadRequest_WhenCreateKeywordDtoIsNull()
        {
            // Arrange
            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.CreateKeyword(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void KeywordController_CreateKeyword_ReturnsUnprocessableEntity_WhenKeywordAlreadyExists()
        {
            // Arrange
            var createKeyword = new KeywordDto { Id = 1, Word = "TestKeyword" };
            var existingKeyword = new Keyword { Id = 1, Word = "TestKeyword" }; // Assuming keyword already exists

            A.CallTo(() => _keywordRepository.GetKeywords())
                .Returns(new List<Keyword> { existingKeyword });
            var controller = new KeywordController(_keywordRepository, _authService, _mapper);

            // Act
            var result = controller.CreateKeyword(createKeyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422); // UnprocessableEntity
        }

        [Fact]
        public void KeywordController_CreateKeyword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createKeyword = new KeywordDto { Id = 1, Word = null }; // Invalid model state

            var controller = new KeywordController(_keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.CreateKeyword(createKeyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int keywordId = 1;
            var updateKeyword = new KeywordDto { Id = keywordId, Word = "UpdatedKeyword" };
            var oldKeyword = new Keyword { Id = keywordId, Word = "OldKeyword" };

            A.CallTo(() => _keywordRepository.GetKeyword(keywordId)).Returns(oldKeyword);
            A.CallTo(() => _keywordRepository.UpdateKeyword(oldKeyword)).Returns(true);

            var controller = GetKeywordControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateKeyword(keywordId, updateKeyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsBadRequest_WhenUpdateKeywordDtoIsNull()
        {
            // Arrange
            var controller = GetKeywordControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateKeyword(1, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsBadRequest_WhenKeywordIdInDtoDoesNotMatch()
        {
            // Arrange
            var controller = GetKeywordControllerInstanceWithAdminAuthorization();
            var updateKeyword = new KeywordDto { Id = 2, Word = "UpdatedKeyword" };

            // Act
            var result = controller.UpdateKeyword(1, updateKeyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsNotFound_WhenKeywordDoesNotExist()
        {
            // Arrange
            var keywordId = 0;
            var controller = GetKeywordControllerInstanceWithAdminAuthorization();
            A.CallTo(() => _keywordRepository.GetKeyword(keywordId)).Returns(null);

            // Act
            var result = controller.UpdateKeyword(0, new KeywordDto { Id = 0, Word = "UpdatedKeyword" });

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            var controller = GetKeywordControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.UpdateKeyword(1, new KeywordDto { Id = 1, Word = "UpdatedKeyword" });

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = GetKeywordControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.UpdateKeyword(1, new KeywordDto { Id = 1, Word = "UpdatedKeyword" });

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void KeywordController_UpdateKeyword_ReturnsInternalServerError_WhenUpdateFails()
        {
            // Arrange
            int keywordId = 1;
            var updateKeyword = new KeywordDto { Id = keywordId, Word = "UpdatedKeyword" };
            var oldKeyword = new Keyword { Id = keywordId, Word = "OldKeyword" };

            A.CallTo(() => _keywordRepository.GetKeyword(keywordId)).Returns(oldKeyword);
            A.CallTo(() => _keywordRepository.UpdateKeyword(oldKeyword)).Returns(false);

            var controller = GetKeywordControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateKeyword(keywordId, updateKeyword);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500); // InternalServerError
        }

        private KeywordController GetKeywordControllerInstanceWithAdminAuthorization()
        {
            var _mapper = A.Fake<IMapper>();
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(true);

            return new KeywordController(_keywordRepository, authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        private KeywordController GetKeywordControllerInstanceWithoutAdminAuthorization()
        {
            var _mapper = A.Fake<IMapper>();
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(false);

            return new KeywordController(_keywordRepository, authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        [Fact]
        public void KeywordController_DeleteKeyword_ReturnsOk_WhenDeleteIsSuccessful()
        {
            // Arrange
            int keywordId = 1;
            var keyword = new Keyword { Id = keywordId, Word = "TestKeyword" };

            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(true);
            A.CallTo(() => _keywordRepository.GetKeyword(keywordId)).Returns(keyword);
            A.CallTo(() => _keywordRepository.DeleteKeyword(keyword)).Returns(true);

            var controller = GetKeywordControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void KeywordController_DeleteKeyword_ReturnsNotFound_WhenKeywordDoesNotExist()
        {
            // Arrange
            int keywordId = 1;

            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(false);
            var controller = GetKeywordControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void KeywordController_DeleteKeyword_ReturnsForbidden_WhenUserIsNotAdmin()
        {
            // Arrange
            int keywordId = 1;
            var controller = GetKeywordControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.DeleteKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void KeywordController_DeleteKeyword_ReturnsInternalServerError_WhenDeleteFails()
        {
            // Arrange
            int keywordId = 1;
            var keyword = new Keyword { Id = keywordId, Word = "TestKeyword" };

            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(true);
            A.CallTo(() => _keywordRepository.GetKeyword(keywordId)).Returns(keyword);
            A.CallTo(() => _keywordRepository.DeleteKeyword(keyword)).Returns(false);

            var controller = GetKeywordControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500); // InternalServerError
        }

        [Fact]
        public void KeywordController_DeleteKeyword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int keywordId = 1;
            var controller = GetKeywordControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("key", "error message");
            A.CallTo(() => _keywordRepository.KeywordExists(keywordId)).Returns(true);

            // Act
            var result = controller.DeleteKeyword(keywordId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
