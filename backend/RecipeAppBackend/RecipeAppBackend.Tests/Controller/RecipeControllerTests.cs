using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Controllers;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using RecipeAppBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppBackend.Tests.Controller
{
    public class RecipeControllerTests
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IKeywordRepository _keywordRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly DefaultHttpContext _httpContext;

        public RecipeControllerTests()
        {
            _recipeRepository = A.Fake<IRecipeRepository>();
            _userRepository = A.Fake<IUserRepository>();
            _imageRepository = A.Fake<IImageRepository>();
            _ingredientRepository = A.Fake<IIngredientRepository>();
            _keywordRepository = A.Fake<IKeywordRepository>();
            _authService = A.Fake<IAuthService>();
            _mapper = A.Fake<IMapper>();
            _httpContext = new DefaultHttpContext();
        }

        private RecipeController GetRecipeControllerInstanceWithAdminAuthorization()
        {
            var _mapper = A.Fake<IMapper>();
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(true);

            return new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        private RecipeController GetRecipeControllerInstanceWithoutAdminAuthorization()
        {
            var _mapper = A.Fake<IMapper>();
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(false);

            return new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        [Fact]
        public void RecipeController_GetRecipes_ReturnsOk_WithRecipes()
        {
            // Arrange
            var recipes = new List<Recipe>
            {
                new Recipe { Id = 1, Name = "Recipe 1" },
                new Recipe { Id = 2, Name = "Recipe 2" },
                new Recipe { Id = 3, Name = "Recipe 3" }
            };
            var recipesDto = new List<RecipeDto>
            {
                new RecipeDto { Id = 1, Name = "Recipe 1" },
                new RecipeDto { Id = 2, Name = "Recipe 2" },
                new RecipeDto { Id = 3, Name = "Recipe 3" }
            };

            A.CallTo(() => _recipeRepository.GetRecipes()).Returns(recipes);
            A.CallTo(() => _mapper.Map<List<RecipeDto>>(_recipeRepository.GetRecipes())).Returns(recipesDto);
            A.CallTo(() => _recipeRepository.GetIngredientsOfRecipe(A<int>._)).Returns(new List<Ingredient>());
            A.CallTo(() => _recipeRepository.GetKeywordsOfRecipe(A<int>._)).Returns(new List<Keyword>());
            A.CallTo(() => _recipeRepository.GetReviewsOfRecipe(A<int>._)).Returns(new List<Review>());
            A.CallTo(() => _recipeRepository.GetImagesOfRecipe(A<int>._)).Returns(new List<Image>());
            A.CallTo(() => _recipeRepository.GetFavoriteCount(A<int>._)).Returns(0);
            A.CallTo(() => _recipeRepository.GetRating(A<int>._)).Returns(0);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetRecipes();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<IEnumerable<RecipeDto>>();
            var recipeDtos = okResult.Value as IEnumerable<RecipeDto>;
            recipeDtos.Should().NotBeNull();
            recipeDtos.Should().HaveCount(3); // Ensure all recipes are returned
        }

        [Fact]
        public void RecipeController_GetRecipes_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.GetRecipes();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void RecipeController_GetRecipe_ReturnsOk_WithRecipe()
        {
            // Arrange
            int recipeId = 1;
            var recipe = new Recipe { Id = recipeId, Name = "Test Recipe" };
            var ingredientDtos = new List<IngredientDto>();
            var keywordDtos = new List<KeywordDto>();
            var reviewDtos = new List<ReviewDto>();
            var imageDtos = new List<ImageDto>();

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);
            A.CallTo(() => _recipeRepository.GetIngredientsOfRecipe(recipeId)).Returns(new List<Ingredient>());
            A.CallTo(() => _recipeRepository.GetKeywordsOfRecipe(recipeId)).Returns(new List<Keyword>());
            A.CallTo(() => _recipeRepository.GetReviewsOfRecipe(recipeId)).Returns(new List<Review>());
            A.CallTo(() => _recipeRepository.GetImagesOfRecipe(recipeId)).Returns(new List<Image>());
            A.CallTo(() => _recipeRepository.GetFavoriteCount(recipeId)).Returns(0);
            A.CallTo(() => _recipeRepository.GetRating(recipeId)).Returns(0);
            A.CallTo(() => _mapper.Map<RecipeDto>(recipe)).Returns(new RecipeDto
            {
                Id = recipe.Id,
                Name = recipe.Name,
                Ingredients = ingredientDtos,
                Keywords = keywordDtos,
                Reviews = reviewDtos,
                Images = imageDtos,
                FavoriteCount = 0,
                Rating = 0
            });

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<RecipeDto>();
            var recipeDto = okResult.Value as RecipeDto;
            recipeDto.Should().NotBeNull();
            recipeDto.Id.Should().Be(recipeId); // Ensure correct recipe is returned
        }

        [Fact]
        public void RecipeController_GetRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
        {
            // Arrange
            int recipeId = 1;
            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_GetRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int recipeId = 1;
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");
            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);

            // Act
            var result = controller.GetRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void RecipeController_GetIngredientsOfRecipe_ReturnsOk_WithIngredients()
        {
            // Arrange
            int recipeId = 1;
            var ingredients = new List<Ingredient>
            {
                new Ingredient { Id = 1, Name = "Ingredient 1" },
                new Ingredient { Id = 2, Name = "Ingredient 2" },
                new Ingredient { Id = 3, Name = "Ingredient 3" }
            };
            var ingredientsDto = new List<IngredientDto>
            {
                new IngredientDto { Id = 1, Name = "Ingredient 1" },
                new IngredientDto { Id = 2, Name = "Ingredient 2" },
                new IngredientDto { Id = 3, Name = "Ingredient 3" }
            };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetIngredientsOfRecipe(recipeId)).Returns(ingredients);
            A.CallTo(() => _mapper.Map<List<IngredientDto>>(A<IEnumerable<Ingredient>>._)).Returns(ingredientsDto);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetIngredientsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<IEnumerable<IngredientDto>>();
            var ingredientDtos = okResult.Value as IEnumerable<IngredientDto>;
            ingredientDtos.Should().NotBeNull();
            ingredientDtos.Should().HaveCount(3); // Ensure all ingredients are returned
        }

        [Fact]
        public void RecipeController_GetIngredientsOfRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
        {
            // Arrange
            int recipeId = 1;

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetIngredientsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_GetIngredientsOfRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int recipeId = 1;
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");
            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);

            // Act
            var result = controller.GetIngredientsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void RecipeController_GetImagesOfRecipe_ReturnsOk_WithImages()
        {
            // Arrange
            int recipeId = 1;
            var images = new List<Image>
            {
                new Image { Id = 1 },
                new Image { Id = 2 },
                new Image { Id = 3 }
            };
            var imagesDto = new List<ImageDto>
            {
                new ImageDto { Id = 1 },
                new ImageDto { Id = 2 },
                new ImageDto { Id = 3 }
            };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetImagesOfRecipe(recipeId)).Returns(images);
            A.CallTo(() => _mapper.Map<List<ImageDto>>(A<IEnumerable<Image>>._)).Returns(imagesDto);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetImagesOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<IEnumerable<ImageDto>>();
            var imageDtos = okResult.Value as IEnumerable<ImageDto>;
            imageDtos.Should().NotBeNull();
            imageDtos.Should().HaveCount(3); // Ensure all images are returned
        }

        [Fact]
        public void RecipeController_GetImagesOfRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
        {
            // Arrange
            int recipeId = 1;

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetImagesOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_GetImagesOfRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int recipeId = 1;
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");
            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);

            // Act
            var result = controller.GetImagesOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void RecipeController_GetKeywordsOfRecipe_ReturnsOk_WithKeywords()
        {
            // Arrange
            int recipeId = 1;
            var keywords = new List<Keyword>
            {
                new Keyword { Id = 1, Word = "Keyword 1" },
                new Keyword { Id = 2, Word = "Keyword 2" },
                new Keyword { Id = 3, Word = "Keyword 3" }
            };
            var keywordsDto = new List<KeywordDto>
            {
                new KeywordDto { Id = 1, Word = "Keyword 1" },
                new KeywordDto { Id = 2, Word = "Keyword 2" },
                new KeywordDto { Id = 3, Word = "Keyword 3" }
            };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetKeywordsOfRecipe(recipeId)).Returns(keywords);
            A.CallTo(() => _mapper.Map<List<KeywordDto>>(A<IEnumerable<Keyword>>._)).Returns(keywordsDto);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetKeywordsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<IEnumerable<KeywordDto>>();
            var keywordDtos = okResult.Value as IEnumerable<KeywordDto>;
            keywordDtos.Should().NotBeNull();
            keywordDtos.Should().HaveCount(3); // Ensure all keywords are returned
        }

        [Fact]
        public void RecipeController_GetKeywordsOfRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
        {
            // Arrange
            int recipeId = 1;

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetKeywordsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_GetKeywordsOfRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int recipeId = 1;
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");
            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);

            // Act
            var result = controller.GetKeywordsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_GetReviewsOfRecipe_ReturnsOk_WithReviews()
        {
            // Arrange
            int recipeId = 1;
            var reviews = new List<Review>
            {
                new Review { Id = 1, Rating = 5, Comment = "Excellent recipe" },
                new Review { Id = 2, Rating = 4, Comment = "Great recipe" },
                new Review { Id = 3, Rating = 3, Comment = "Good recipe" }
            };
            var reviewsDto = new List<ReviewDto>
            {
                new ReviewDto { Id = 1, Rating = 5, Comment = "Excellent recipe" },
                new ReviewDto { Id = 2, Rating = 4, Comment = "Great recipe" },
                new ReviewDto { Id = 3, Rating = 3, Comment = "Good recipe" }
            };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetReviewsOfRecipe(recipeId)).Returns(reviews);
            A.CallTo(() => _mapper.Map<List<ReviewDto>>(A<IEnumerable<Review>>._)).Returns(reviewsDto);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetReviewsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().BeAssignableTo<IEnumerable<ReviewDto>>();
            var reviewDtos = okResult.Value as IEnumerable<ReviewDto>;
            reviewDtos.Should().NotBeNull();
            reviewDtos.Should().HaveCount(3); // Ensure all reviews are returned
        }

        [Fact]
        public void RecipeController_GetReviewsOfRecipe_ReturnsNotFound_WhenRecipeDoesNotExist()
        {
            // Arrange
            int recipeId = 1;

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.GetReviewsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_GetReviewsOfRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int recipeId = 1;
            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);
            controller.ModelState.AddModelError("key", "error message");
            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);

            // Act
            var result = controller.GetReviewsOfRecipe(recipeId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_CreateRecipe_ReturnsOk_WhenSuccessful()
        {
            // Arrange
            var createRecipeDto = new RecipeDto { Id = 1, Name = "name", UserId = 1};
            var expectedMessage = "Successfully created";

            // Mock AuthService to return a valid user ID
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns("valid_user_id");

            // Mock UserRepository to return a user
            A.CallTo(() => _userRepository.GetUser(A<int>._)).Returns(new User { Id = 1 });

            // Mock RecipeRepository to return true for successful recipe creation
            A.CallTo(() => _recipeRepository.CreateRecipe(A<Recipe>._, A<List<RecipeKeyword>>._)).Returns(true);

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authUserId = "1"; // Assuming the user is authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.CreateRecipe(createRecipeDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();

            var okResult = result as OkObjectResult;
            okResult.Value.Should().Be(expectedMessage);
        }

        [Fact]
        public void RecipeController_CreateRecipe_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createRecipeDto = new RecipeDto { Id = 1, Name = "name" };
            var controller = GetRecipeControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.CreateRecipe(createRecipeDto);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_AddKeyword_Returns_Forbidden_When_User_Not_Authorized()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("2");

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.AddKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void RecipeController_AddKeyword_Returns_NotFound_When_Recipe_Not_Found()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(null);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.AddKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_AddKeyword_Returns_BadRequest_When_Keyword_Is_Empty()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "";
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.AddKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_AddKeyword_Returns_BadRequest_When_Keyword_Creation_Fails()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);
            A.CallTo(() => _keywordRepository.GetKeywords()).Returns(new List<Keyword>());
            A.CallTo(() => _keywordRepository.CreateKeyword(A<Keyword>._)).Returns(false);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.AddKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(500);
        }


        [Fact]
        public void RecipeController_UpdateRecipe_Returns_BadRequest_When_RequestBody_Is_Null()
        {
            // Arrange
            var recipeId = 1;
            RecipeDto updateRecipe = null;

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.UpdateRecipe(recipeId, updateRecipe);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_UpdateRecipe_Returns_BadRequest_When_RecipeId_Mismatch_Between_Path_And_Body()
        {
            // Arrange
            var recipeId = 1;
            var updateRecipe = new RecipeDto { Id = 2 };

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.UpdateRecipe(recipeId, updateRecipe);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_UpdateRecipe_Returns_NotFound_When_Recipe_Not_Found()
        {
            // Arrange
            var recipeId = 1;
            var updateRecipe = new RecipeDto { Id = recipeId };

            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(null);

            var controller = new RecipeController(_recipeRepository, _userRepository, _imageRepository, _ingredientRepository, _keywordRepository, _authService, _mapper);

            // Act
            var result = controller.UpdateRecipe(recipeId, updateRecipe);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_UpdateRecipe_Returns_Forbidden_When_User_Not_Authorized()
        {
            // Arrange
            var recipeId = 1;
            var updateRecipe = new RecipeDto { Id = recipeId };
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("2");

            var controller = GetRecipeControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.UpdateRecipe(recipeId, updateRecipe);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void RecipeController_UpdateRecipe_Returns_BadRequest_When_User_Id_Invalid()
        {
            // Arrange
            var recipeId = 1;
            var updateRecipe = new RecipeDto { Id = recipeId, UserId = 2 };
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);
            A.CallTo(() => _userRepository.GetUser(updateRecipe.UserId)).Returns(null);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateRecipe(recipeId, updateRecipe);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422);
        }


        [Fact]
        public void RecipeController_DeleteRecipe_Returns_NotFound_When_Recipe_Not_Found()
        {
            // Arrange
            var recipeId = 1;

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteRecipe(recipeId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RecipeController_DeleteRecipe_Returns_Forbidden_When_User_Not_Authorized()
        {
            // Arrange
            var recipeId = 1;
            var deleteRecipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(deleteRecipe);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("2");

            var controller = GetRecipeControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.DeleteRecipe(recipeId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void RecipeController_DeleteRecipe_Returns_BadRequest_When_ModelState_Is_Invalid()
        {
            // Arrange
            var recipeId = 1;
            var deleteRecipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(deleteRecipe);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error");

            // Act
            var result = controller.DeleteRecipe(recipeId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RecipeController_DeleteRecipe_Returns_InternalServerError_When_Repository_Fails_To_Delete()
        {
            // Arrange
            var recipeId = 1;
            var deleteRecipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(deleteRecipe);
            A.CallTo(() => _recipeRepository.DeleteRecipe(deleteRecipe)).Returns(false);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteRecipe(recipeId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public void RecipeController_DeleteRecipe_Returns_Ok_When_Recipe_Deleted_Successfully()
        {
            // Arrange
            var recipeId = 1;
            var deleteRecipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(deleteRecipe);
            A.CallTo(() => _recipeRepository.DeleteRecipe(deleteRecipe)).Returns(true);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteRecipe(recipeId);

            // Assert
            result.Should().BeOfType<OkObjectResult>()
                .Which.StatusCode.Should().Be(200);
        }


        [Fact]
        public void RemoveKeyword_Returns_NotFound_When_Recipe_Not_Found()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(false);

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.RemoveKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void RemoveKeyword_Returns_BadRequest_When_Keyword_Length_Is_Zero()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "";

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);

            // Act
            var result = controller.RemoveKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void RemoveKeyword_Returns_Forbidden_When_User_Not_Authorized()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("2");

            var controller = GetRecipeControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.RemoveKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void RemoveKeyword_Returns_UnprocessableEntity_When_Keyword_Not_Found()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);
            A.CallTo(() => _recipeRepository.GetRecipeKeywordsOfRecipe(recipeId)).Returns(new List<RecipeKeyword>());

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.RemoveKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(422);
        }

        [Fact]
        public void RemoveKeyword_Returns_BadRequest_When_ModelState_Is_Invalid()
        {
            // Arrange
            var recipeId = 1;
            var keyword = "test";
            var recipe = new Recipe { Id = recipeId, User = new User { Id = 1 } };
            var recipeKeywords = new List<RecipeKeyword> { new RecipeKeyword { Keyword = new Keyword { Word = "test" } } };

            A.CallTo(() => _recipeRepository.RecipeExists(recipeId)).Returns(true);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);
            A.CallTo(() => _recipeRepository.GetRecipeKeywordsOfRecipe(recipeId)).Returns(recipeKeywords);

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            var controller = GetRecipeControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error");

            // Act
            var result = controller.RemoveKeyword(recipeId, keyword);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
