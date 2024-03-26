using AutoMapper;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Repositories;
using RecipeAppBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Controllers;
using FluentAssertions;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace RecipeAppBackend.Tests.Controller
{
    public class UserControllerTests
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public UserControllerTests()
        {
            _userRepository = A.Fake<IUserRepository>();
            _recipeRepository = A.Fake<IRecipeRepository>();
            _authService = A.Fake<IAuthService>();
            _mapper = A.Fake<IMapper>();
        }

        private UserController GetUserControllerInstanceWithAdminAuthorization()
        {
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(true);
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            return new UserController(_userRepository,_recipeRepository, authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        private UserController GetUserControllerInstanceWithoutAdminAuthorization()
        {
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(false);

            return new UserController(_userRepository, _recipeRepository, authService, _mapper)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }


        [Fact]
        public void UserController_GetUsers_ReturnsListOfUsers_WhenUsersExist()
        {
            // Arrange
            var users = new List<User> {
                new User { Id = 1, Name = "User1", Email = "user1@example.com" },
                new User { Id = 2, Name = "User2", Email = "user2@example.com" }
            };
            var usersDto = new List<UserDto> {
                new UserDto { Id = 1, Name = "User1", Email = "user1@example.com" },
                new UserDto { Id = 2, Name = "User2", Email = "user2@example.com" }
            };
            A.CallTo(() => _userRepository.GetUsers()).Returns(users);
            A.CallTo(() => _mapper.Map<List<UserDto>>(users)).Returns(usersDto);

            var controller = GetUserControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.GetUsers() as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var userList = result.Value as List<UserDto>;
            userList.Should().NotBeNull();
            userList.Should().HaveCount(2);
            userList.Select(u => u.Id).Should().Contain(users.Select(u => u.Id));
            userList.Select(u => u.Name).Should().Contain(users.Select(u => u.Name));
            userList.Select(u => u.Email).Should().Contain(users.Select(u => u.Email));
        }

        [Fact]
        public void UserController_GetUsers_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error Message");

            // Act
            var result = controller.GetUsers() as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }


        [Fact]
        public void UserController_GetUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var user = new User { Id = userId, Name = "User1", Email = "user1@example.com" };
            var fakeUserDto = new UserDto { Id = userId, Name = "User1", Email = "user1@example.com" };

            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _mapper.Map<UserDto>(user)).Returns(fakeUserDto);
            var controller = GetUserControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.GetUser(userId) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var userDto = result.Value as UserDto;
            userDto.Should().NotBeNull();
            userDto.Id.Should().Be(userId);
            userDto.Name.Should().Be(user.Name);
            userDto.Email.Should().Be(user.Email);
        }

        [Fact]
        public void UserController_GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(false);
            var controller = GetUserControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.GetUser(userId) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void UserController_GetUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var userId = 1;
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error Message");
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);

            // Act
            var result = controller.GetUser(userId) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }

        [Fact]
        public void UserController_GetUsersFavorites_ReturnsRecipes_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var recipes = new List<Recipe>
            {
                new Recipe { Id = 1, Name = "Recipe1" },
                new Recipe { Id = 2, Name = "Recipe2" }
            };
            var recipesDto = new List<RecipeDto>
            {
                new RecipeDto { Id = 1, Name = "Recipe1" },
                new RecipeDto { Id = 2, Name = "Recipe2" }
            };
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);
            A.CallTo(() => _userRepository.GetUsersFavorites(userId)).Returns(recipes);
            A.CallTo(() => _mapper.Map<List<RecipeDto>>(recipes)).Returns(recipesDto);
            var controller = new UserController(_userRepository, _recipeRepository, null, _mapper);

            // Act
            var result = controller.GetUsersFavorites(userId) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var recipeDtos = result.Value as IEnumerable<RecipeDto>;
            recipeDtos.Should().NotBeNull();
            recipeDtos.Should().HaveCount(2);
            recipeDtos.Should().OnlyContain(r => r != null);
        }

        [Fact]
        public void UserController_GetUsersFavorites_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(false);
            var controller = new UserController(_userRepository, _recipeRepository, null, _mapper);

            // Act
            var result = controller.GetUsersFavorites(userId) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }


        [Fact]
        public void UserController_GetUsersFavorites_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var userId = 1;
            var controller = new UserController(_userRepository, _recipeRepository, null, _mapper);
            controller.ModelState.AddModelError("Key", "Error Message");
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);

            // Act
            var result = controller.GetUsersFavorites(userId) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }


        [Fact]
        public void UserController_GetUsersRecipes_ReturnsRecipes_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var recipes = new List<Recipe>
            {
                new Recipe { Id = 1, Name = "Recipe1" },
                new Recipe { Id = 2, Name = "Recipe2" }
            };
            var recipesDto = new List<RecipeDto>
            {
                new RecipeDto { Id = 1, Name = "Recipe1" },
                new RecipeDto { Id = 2, Name = "Recipe2" }
            };
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);
            A.CallTo(() => _userRepository.GetUsersRecipes(userId)).Returns(recipes);
            A.CallTo(() => _mapper.Map<List<RecipeDto>>(recipes)).Returns(recipesDto);
            var controller = new UserController(_userRepository, _recipeRepository, null, _mapper);

            // Act
            var result = controller.GetUsersRecipes(userId) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var resultRecipes = result.Value as List<RecipeDto>;
            resultRecipes.Should().NotBeNull();
            resultRecipes.Should().HaveCount(2);
            resultRecipes[0].Name.Should().Be("Recipe1");
            resultRecipes[1].Name.Should().Be("Recipe2");
        }

        [Fact]
        public void UserController_GetUsersRecipes_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(false);
            var controller = new UserController(_userRepository, _recipeRepository, null, _mapper);

            // Act
            var result = controller.GetUsersRecipes(userId) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void UserController_GetUsersRecipes_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var userId = 1;
            var controller = new UserController(_userRepository, _recipeRepository, null, _mapper);
            controller.ModelState.AddModelError("Key", "Error Message");
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);

            // Act
            var result = controller.GetUsersRecipes(userId) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }


        [Fact]
        public void UserController_GetUsersReviews_ReturnsReviews_WhenUserExists()
        {
            // Arrange
            var userId = 1;
            var reviews = new List<Review>
            {
                new Review { Id = 1, Rating = 4, Comment = "Good review" },
                new Review { Id = 2, Rating = 5, Comment = "Excellent review" }
            };
            var reviewsDto = new List<ReviewDto>
            {
                new ReviewDto { Id = 1, Rating = 4, Comment = "Good review" },
                new ReviewDto { Id = 2, Rating = 5, Comment = "Excellent review" }
            };
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);
            A.CallTo(() => _userRepository.GetUsersReviews(userId)).Returns(reviews);
            A.CallTo(() => _mapper.Map<List<ReviewDto>>(reviews)).Returns(reviewsDto);
            var controller = new UserController(_userRepository, null, null, _mapper);

            // Act
            var result = controller.GetUsersReviews(userId) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);

            var resultReviews = result.Value as List<ReviewDto>;
            resultReviews.Should().NotBeNull();
            resultReviews.Should().HaveCount(2);
            resultReviews[0].Rating.Should().Be(4);
            resultReviews[0].Comment.Should().Be("Good review");
            resultReviews[1].Rating.Should().Be(5);
            resultReviews[1].Comment.Should().Be("Excellent review");
        }

        [Fact]
        public void UserController_GetUsersReviews_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = 1;
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(false);
            var controller = new UserController(_userRepository, null, null, _mapper);

            // Act
            var result = controller.GetUsersReviews(userId) as NotFoundResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }

        [Fact]
        public void UserController_GetUsersReviews_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var userId = 1;
            var controller = new UserController(_userRepository, null, null, _mapper);
            controller.ModelState.AddModelError("Key", "Error Message");
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);

            // Act
            var result = controller.GetUsersReviews(userId) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }


        [Fact]
        public void UserController_RecoverPassword_ReturnsNoContent_WhenEmailExists()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Email = email, Password = "password123" };
            A.CallTo(() => _userRepository.GetUserByEmail(email)).Returns(user);
            var controller = new UserController(_userRepository, null, _authService, null);

            // Act
            var result = controller.RecoverPassword(email) as NoContentResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(204);
            A.CallTo(() => _authService.RestorePassword(email, user.Password)).MustHaveHappenedOnceExactly();
        }

        [Fact]
        public void UserController_RecoverPassword_ReturnsUnprocessableEntity_WhenEmailDoesNotExist()
        {
            // Arrange
            var email = "nonexistent@example.com";
            A.CallTo(() => _userRepository.GetUserByEmail(email)).Returns(null);
            var controller = new UserController(_userRepository, null, _authService, null);

            // Act
            var result = controller.RecoverPassword(email) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(422);
            result.Value.Should().BeOfType<ModelStateDictionary>();
            A.CallTo(() => _authService.RestorePassword(A<string>._, A<string>._)).MustNotHaveHappened();
        }

        [Fact]
        public void UserController_RecoverPassword_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var email = "test@example.com";
            var user = new User { Id = 1, Email = email, Password = "password123", Username = "username", Name ="name" };
            A.CallTo(() => _userRepository.GetUserByEmail(email)).Returns(user);
            var controller = new UserController(_userRepository, null, _authService, null);
            controller.ModelState.AddModelError("Key", "Error Message");

            // Act
            var result = controller.RecoverPassword(email) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
            A.CallTo(() => _authService.RestorePassword(A<string>._, A<string>._)).MustNotHaveHappened();
        }


        [Fact]
        public void UserController_CreateUser_ReturnsOk_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var createUserDto = new CreateUserDto
            {
                Username = "testuser",
                Email = "test@example.com",
                Password = "password123",
                Admin = false // Ensure new user is not an admin
            };
            var user = new User { Username = "testuser", Email = "test@example.com" };
            A.CallTo(() => _userRepository.ValidateEmail(createUserDto.Email)).Returns(true);
            A.CallTo(() => _userRepository.GetUsers()).Returns(new List<User>());
            A.CallTo(() => _mapper.Map<User>(createUserDto)).Returns(user);
            A.CallTo(() => _authService.EncryptString(createUserDto.Password)).Returns(createUserDto.Password); // Fake encryption for testing
            A.CallTo(() => _userRepository.CreateUser(user)).Returns(true);
            var controller = new UserController(_userRepository, null, _authService, _mapper);

            // Act
            var result = controller.CreateUser(createUserDto) as OkObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().Be("Succesfully created");
        }

        [Theory]
        [InlineData("existing@example.com")]
        [InlineData("test@example.com")]
        public void UserController_CreateUser_ReturnsUnprocessableEntity_WhenEmailIsInvalidOrAlreadyExists(string email)
        {
            // Arrange
            var createUserDto = new CreateUserDto { Username = "testuser", Email = email, Password = "password123", Admin = false };
            var user = new User { Email = email };
            A.CallTo(() => _userRepository.ValidateEmail(createUserDto.Email)).Returns(false);
            A.CallTo(() => _userRepository.GetUsers()).Returns(new List<User> { user });
            var controller = GetUserControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.CreateUser(createUserDto) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(422);
            result.Value.Should().BeOfType<ModelStateDictionary>();
        }

        [Fact]
        public void UserController_CreateUser_ReturnsUnprocessableEntity_WhenUsernameAlreadyExists()
        {
            // Arrange
            var username = "existinguser";
            var createUserDto = new CreateUserDto { Username = username, Email = "test@example.com", Password = "password123", Admin = false };
            var user = new User { Username = username };
            A.CallTo(() => _userRepository.ValidateEmail(createUserDto.Email)).Returns(true);
            A.CallTo(() => _userRepository.GetUsers()).Returns(new List<User> { user });
            var controller = new UserController(_userRepository, null, _authService, _mapper);

            // Act
            var result = controller.CreateUser(createUserDto) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(422);
            result.Value.Should().BeOfType<ModelStateDictionary>();
        }

        [Fact]
        public void UserController_CreateUser_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var createUserDto = A.Fake<CreateUserDto>();
            var controller = new UserController(_userRepository, null, _authService, _mapper);
            controller.ModelState.AddModelError("Key", "Error Message");
            A.CallTo(() => _userRepository.ValidateEmail(A<string>._)).Returns(true);

            // Act
            var result = controller.CreateUser(createUserDto) as BadRequestObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(400);
        }


        //[Fact]
        //public void UserController_Login_ReturnsOkObjectResult_WhenUserExists()
        //{
        //    // Arrange
        //    var loginUser = new LoginUserDto { Username = "testuser", Password = "password123" };
        //    var user = new User { Id = 1, Username = loginUser.Username, Password = "password123" };
        //    A.CallTo(() => _userRepository.GetUserByUsername(loginUser.Username)).Returns(user);
        //    A.CallTo(() => _authService.VerifyPassword(loginUser.Password, user.Password)).Returns(true);
        //    var controller = new UserController(_userRepository, null, _authService, null);

        //    A.CallTo(() => _authService.GenerateToken(user)).Returns("adw");

        //    // Act
        //    var result = controller.Login(loginUser) as OkObjectResult;

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.StatusCode.Should().Be(200);
        //}

        [Fact]
        public void UserController_Login_ReturnsUnprocessableEntity_WhenUserDoesNotExist()
        {
            // Arrange
            var loginUser = new LoginUserDto { Username = "nonexistent", Password = "password123" };
            A.CallTo(() => _userRepository.GetUsers()).Returns(new List<User>());
            var controller = new UserController(_userRepository, null, _authService, null);

            // Act
            var result = controller.Login(loginUser) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(422);
            result.Value.Should().BeOfType<ModelStateDictionary>();
        }

        [Fact]
        public void UserController_Login_ReturnsUnprocessableEntity_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginUser = new LoginUserDto { Username = "testuser", Password = "wrongpassword" };
            var user = new User { Id = 1, Username = loginUser.Username, Password = "password123" };
            A.CallTo(() => _userRepository.GetUsers()).Returns(new List<User> { user });
            A.CallTo(() => _authService.VerifyPassword(loginUser.Password, user.Password)).Returns(false);
            var controller = new UserController(_userRepository, null, _authService, null);

            // Act
            var result = controller.Login(loginUser) as ObjectResult;

            // Assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(422);
            result.Value.Should().BeOfType<ModelStateDictionary>();
        }

        //[Fact]
        //public void UserController_Login_ReturnsBadRequest_WhenModelStateIsInvalid()
        //{
        //    // Arrange
        //    var loginUser = new LoginUserDto { Username = "testuser", Password = "password123" };
        //    var user = A.Fake<User>();
        //    var controller = new UserController(_userRepository, null, _authService, null);
        //    controller.ModelState.AddModelError("Key", "Error Message");
        //    A.CallTo(() => _userRepository.GetUserByUsername(loginUser.Username)).Returns(user);
        //    A.CallTo(() => _authService.VerifyPassword(loginUser.Password, user.Password)).Returns(true);

        //    // Act
        //    var result = controller.Login(loginUser) as BadRequestObjectResult;

        //    // Assert
        //    result.Should().NotBeNull();
        //    result.StatusCode.Should().Be(400);
        //}


        [Fact]
        public void UserController_AddFavorite_Returns_Forbid_When_UserIdsDoNotMatch()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithoutAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var authUserId = "2";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);

            // Act
            var result = controller.AddFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void UserController_AddFavorite_Returns_NotFound_When_UserDoesNotExist()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var authUserId = "1";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(null);

            // Act
            var result = controller.AddFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public void UserController_AddFavorite_Returns_NotFound_When_RecipeDoesNotExist()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var authUserId = "1";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(new User());
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(null);

            // Act
            var result = controller.AddFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public void UserController_AddFavorite_Returns_UnprocessableEntity_When_FavoriteAlreadyExists()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var authUserId = "1";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(new User());
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(new Recipe());
            A.CallTo(() => _userRepository.FavoriteExists(userId, recipeId)).Returns(true);

            // Act
            var result = controller.AddFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422);
        }

        [Fact]
        public void UserController_AddFavorite_Returns_BadRequest_When_ModelStateIsNotValid()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var authUserId = "1";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(new User());
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(new Recipe());
            A.CallTo(() => _userRepository.FavoriteExists(userId, recipeId)).Returns(false);
            controller.ModelState.AddModelError("TestError", "TestErrorMessage");

            // Act
            var result = controller.AddFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void UserController_UpdateUser_Returns_Forbid_When_UserIdsDoNotMatch()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithoutAdminAuthorization();
            var userId = 1;
            var updateUser = new CreateUserDto { Id = 2 };
            var authUserId = "3";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);

            // Act
            var result = controller.UpdateUser(userId, updateUser);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void UserController_UpdateUser_Returns_NotFound_When_UserDoesNotExist()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var updateUser = new CreateUserDto();
            var authUserId = "1";
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(null);

            // Act
            var result = controller.UpdateUser(userId, updateUser);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UserController_UpdateUser_Returns_BadRequest_When_UpdateUserIsNull()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;

            // Act
            var result = controller.UpdateUser(userId, null);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void UserController_DeleteUser_Returns_Forbid_When_UserIdsDoNotMatch()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithoutAdminAuthorization();
            var userId = 1;
            var token = "invalid_token";
            A.CallTo(() => _authService.GetUserId(token)).Returns("2");
            A.CallTo(() => _authService.IsAdmin(token)).Returns(false);

            // Act
            var result = controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void UserController_DeleteUser_Returns_NotFound_When_UserDoesNotExist()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var token = "valid_token";
            A.CallTo(() => _authService.GetUserId(token)).Returns("1");
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(false);

            // Act
            var result = controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void UserController_DeleteUser_Returns_BadRequest_When_ModelStateIsInvalid()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var token = "valid_token";
            A.CallTo(() => _authService.GetUserId(token)).Returns("1");
            A.CallTo(() => _userRepository.UserExists(userId)).Returns(true);
            controller.ModelState.AddModelError("TestError", "Test Error Message");

            // Act
            var result = controller.DeleteUser(userId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void UserController_RemoveFavorite_Returns_Forbid_When_UserIdsDoNotMatch()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithoutAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var token = "invalid_token";
            A.CallTo(() => _authService.GetUserId(token)).Returns("2");

            // Act
            var result = controller.RemoveFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void UserController_RemoveFavorite_Returns_NotFound_When_UserDoesNotExist()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var token = "valid_token";
            A.CallTo(() => _authService.GetUserId(token)).Returns("1");
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(null as User);

            // Act
            var result = controller.RemoveFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public void UserController_RemoveFavorite_Returns_NotFound_When_RecipeDoesNotExist()
        {
            // Arrange
            var controller = GetUserControllerInstanceWithAdminAuthorization();
            var userId = 1;
            var recipeId = 1;
            var token = "valid_token";
            var user = new User { Id = userId };
            A.CallTo(() => _authService.GetUserId(token)).Returns("1");
            A.CallTo(() => _userRepository.GetUser(userId)).Returns(user);
            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(null as Recipe);

            // Act
            var result = controller.RemoveFavorite(userId, recipeId);

            // Assert
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(404);
        }
    }
}
