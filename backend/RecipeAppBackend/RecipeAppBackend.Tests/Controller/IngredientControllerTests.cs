using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Controllers;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppBackend.Tests.Controller
{
    public class IngredientControllerTests
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;
        private readonly DefaultHttpContext _httpContext;

        public IngredientControllerTests()
        {
            _ingredientRepository = A.Fake<IIngredientRepository>();
            _mapper = A.Fake<IMapper>();
            _recipeRepository = A.Fake<IRecipeRepository>();
            _authService = A.Fake<IAuthService>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void IngredientController_GetIngredients_ReturnsOK()
        {
            //Arrange
            var ingedients = A.Fake<ICollection<Ingredient>>();
            var ingredientsDto = A.Fake<List<IngredientDto>>();

            A.CallTo(() => _ingredientRepository.GetIngredients()).Returns(ingedients);
            A.CallTo(() => _mapper.Map<List<IngredientDto>>(_ingredientRepository.GetIngredients())).Returns(ingredientsDto);
            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            //Act
            var result = controller.GetIngredients();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }


        [Fact]
        public void IngredientController_GetIngredient_ReturnsOk_WhenIngredientExists()
        {
            // Arrange
            int ingId = 1;
            var ingredient = new Ingredient { Id = ingId, Name = "TestIngredient" };
            var ingredientDto = new IngredientDto { Id = ingId, Name = "TestIngredient" };

            A.CallTo(() => _ingredientRepository.IngredientExists(ingId)).Returns(true);
            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(ingredient);
            A.CallTo(() => _mapper.Map<IngredientDto>(ingredient)).Returns(ingredientDto);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.GetIngredient(ingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(ingredientDto);
        }

        [Fact]
        public void IngredientController_GetIngredient_ReturnsNotFound_WhenIngredientDoesNotExist()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID doesn't exist

            A.CallTo(() => _ingredientRepository.IngredientExists(ingId)).Returns(false);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.GetIngredient(ingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void IngredientController_GetIngredient_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            int ingId = 1;
            var ingredient = new Ingredient { Id = ingId, Name = "TestIngredient" };

            A.CallTo(() => _ingredientRepository.IngredientExists(ingId)).Returns(true);
            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(ingredient);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.GetIngredient(ingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestResult>();
        }


        [Fact]
        public void IngredientController_CreateIngredient_ReturnsOk_WhenCreationIsSuccessful()
        {
            // Arrange
            var ingredientCreate = new IngredientDto { Id = 1, RecipeId = 1, Amount = 0 }; // Assuming this recipe ID exists
            var recipe = new Recipe { Id = 1, Name = "name", Instructions = "asd", User = new User { Id = 1, Username = "username", Password = "pasword", Email = "email", Name = "name" } };


            A.CallTo(() => _recipeRepository.GetRecipe(ingredientCreate.RecipeId)).Returns(recipe);

            // Simulate authorization
            var authUserId = "1"; // Assuming the user is authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.CreateIngredient(ingredientCreate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(ObjectResult));
        }

        [Fact]
        public void IngredientController_CreateIngredient_ReturnsBadRequest_WhenIngredientCreateIsNull()
        {
            // Arrange
            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.CreateIngredient(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void IngredientController_CreateIngredient_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var ingredientCreate = new IngredientDto { RecipeId = 1 }; // Assuming this recipe ID exists
            var recipe = new Recipe { Id = 1, /* Populate other properties as needed */ };

            A.CallTo(() => _recipeRepository.GetRecipe(ingredientCreate.RecipeId)).Returns(recipe);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.CreateIngredient(ingredientCreate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void IngredientController_CreateIngredient_ReturnsUnprocessableEntity_WhenRecipeDoesNotExist()
        {
            // Arrange
            var ingredientCreate = new IngredientDto { RecipeId = 999 }; // Assuming this recipe ID doesn't exist

            A.CallTo(() => _recipeRepository.GetRecipe(ingredientCreate.RecipeId)).Returns(null);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.CreateIngredient(ingredientCreate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422);
        }

        [Fact]
        public void IngredientController_CreateIngredient_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var ingredientCreate = new IngredientDto { Id = 1, RecipeId = 1, Amount = 0 }; // Assuming this recipe ID exists
            var recipe = new Recipe { Id = 1, Name = "name", Instructions = "asd", User = new User { Id = 2, Username = "username", Password = "pasword", Email = "email", Name = "name" } };
            var recipeId = 1;
            
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);

            // Simulate authorization
            var authUserId = "1"; // Assuming the user is not authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _authService.IsAdmin(A<string>._)).Returns(false);
            
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.CreateIngredient(ingredientCreate);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void IngredientController_UpdateIngredient_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            int ingId = 1;
            var updateIngredient = new IngredientDto { Id = ingId };
            var authRecipe = new Recipe { Id = 1, User = new User { Id = 1 } };
            var oldIngredient = new Ingredient { Id = ingId, Recipe = authRecipe };
            

            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(oldIngredient);
            A.CallTo(() => _recipeRepository.GetRecipe(oldIngredient.Recipe.Id)).Returns(authRecipe);
            A.CallTo(() => _ingredientRepository.UpdateIngredient(oldIngredient)).Returns(true);

            // Simulate authorization
            var authUserId = "1"; // Assuming the user is not authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _authService.IsAdmin(A<string>._)).Returns(true);

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            // Act
            var result = controller.UpdateIngredient(ingId, updateIngredient);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void IngredientController_UpdateIngredient_ReturnsBadRequest_WhenUpdateIngredientIsNull()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID exists

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.UpdateIngredient(ingId, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void IngredientController_UpdateIngredient_ReturnsBadRequest_WhenIngredientIdMismatch()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID exists
            var updateIngredient = new IngredientDto { Id = 2 }; // Assuming this ID doesn't match ingId

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.UpdateIngredient(ingId, updateIngredient);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void IngredientController_UpdateIngredient_ReturnsNotFound_WhenIngredientDoesNotExist()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID doesn't exist

            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(null);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.UpdateIngredient(ingId, new IngredientDto());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void IngredientController_UpdateIngredient_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID exists
            var updateIngredient = new IngredientDto { Id = ingId }; // Assuming this ID matches ingId
            var oldIngredient = new Ingredient { Id = ingId, Recipe = new Recipe { Id = 1, User = new User { Id = 2 } } }; // Assuming user ID is different
            var authRecipe = new Recipe { Id = 1, User = new User { Id = 2 } };

            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(oldIngredient);
            A.CallTo(() => _recipeRepository.GetRecipe(oldIngredient.Recipe.Id)).Returns(authRecipe);

            // Simulate authorization
            var authUserId = "1"; // Assuming the user is not authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _authService.IsAdmin(A<string>._)).Returns(false);

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
            // Act
            var result = controller.UpdateIngredient(ingId, updateIngredient);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void IngredientController_DeleteIngredient_ReturnsOk_WhenDeletionIsSuccessful()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID exists
            var recipe = new Recipe { Id = 1, User = new User { Id = 1 } };
            var deleteIngredient = new Ingredient { Id = ingId, Recipe = recipe };

            A.CallTo(() => _ingredientRepository.IngredientExists(ingId)).Returns(true);
            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(deleteIngredient);
            A.CallTo(() => _recipeRepository.GetRecipe(deleteIngredient.Recipe.Id)).Returns(recipe);
            A.CallTo(() => _ingredientRepository.DeleteIngredient(deleteIngredient)).Returns(true);

            // Simulate authorization
            var authUserId = "1";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _authService.IsAdmin(A<string>._)).Returns(false);

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.DeleteIngredient(ingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void IngredientController_DeleteIngredient_ReturnsNotFound_WhenIngredientDoesNotExist()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID doesn't exist

            A.CallTo(() => _ingredientRepository.IngredientExists(ingId)).Returns(false);

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.DeleteIngredient(ingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void IngredientController_DeleteIngredient_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            int ingId = 1; // Assuming this ingredient ID exists
            var recipe = new Recipe { Id = 1, User = new User { Id = 2 } };
            var deleteIngredient = new Ingredient { Id = ingId, Recipe = recipe }; // Assuming user ID is different
            
            A.CallTo(() => _ingredientRepository.IngredientExists(ingId)).Returns(true);
            A.CallTo(() => _ingredientRepository.GetIngredient(ingId)).Returns(deleteIngredient);
            A.CallTo(() => _recipeRepository.GetRecipe(deleteIngredient.Recipe.Id)).Returns(recipe);

            // Simulate authorization
            var authUserId = "1"; // Assuming the user is not authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _authService.IsAdmin(A<string>._)).Returns(false);

            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var controller = new IngredientController(_ingredientRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.DeleteIngredient(ingId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ForbidResult>();
        }
    }

}
