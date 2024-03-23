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
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RecipeAppBackend.Tests.Controller
{
    public class ImageControllerTests
    {
        private readonly IImageRepository _imageRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly HttpContext _httpContext;

        public ImageControllerTests()
        {
            _imageRepository = A.Fake<IImageRepository>();
            _recipeRepository = A.Fake<IRecipeRepository>();
            _authService = A.Fake<IAuthService>();
            _mapper = A.Fake<IMapper>();
            _httpContext = new DefaultHttpContext();
        }

        [Fact]
        public void ImageController_GetImages_ReturnsOK()
        {
            //Arrange
            var images = A.Fake<ICollection<Image>>();
            var imageDtos = A.Fake<List<ImageDto>>();
            A.CallTo(() => _mapper.Map<List<ImageDto>>(images)).Returns(imageDtos);
            
            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            //Act
            var result = controller.GetImages();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void ImageController_GetImages_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.GetImages();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }



        [Fact]
        public void ImageController_GetImage_ReturnsOk_WhenImageExists()
        {
            // Arrange
            var imageId = 1;
            var image = A.Fake<Image>();
            var imageDto = A.Fake<ImageDto>();

            A.CallTo(() => _imageRepository.ImageExists(imageId)).Returns(true);
            A.CallTo(() => _imageRepository.GetImage(imageId)).Returns(image);
            A.CallTo(() => _mapper.Map<ImageDto>(A<Image>.Ignored)).Returns(imageDto);

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.GetImage(imageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));

            var okResult = (OkObjectResult)result;
            okResult.Value.Should().BeEquivalentTo(imageDto);
        }

        [Fact]
        public void ImageController_GetImage_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var nonExistentImageId = 999; // Assuming this ID doesn't exist
            A.CallTo(() => _imageRepository.ImageExists(nonExistentImageId)).Returns(false);

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.GetImage(nonExistentImageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }



        [Fact]
        public void ImageController_CreateImage_ReturnsOk_WhenImageCreationIsSuccessful()
        {
            // Arrange
            var createImage = new ImageDto
            {
                Id = 1,
                RecipeId = 1
            }; // Assuming this recipe ID exists
            var recipe = new Recipe {
                Id = 1,
                Name = "asd",
                Instructions = "asd",
                Visibility = true,
                User = new User {
                    Id = 1,
                    Password = "password",
                    Username = "Username",
                    Name = "name",
                    Email = "email",
                    Recipes = [],
                    Reviews = [],
                    Favorites = []
                } 
            };
            int recipeId = 1;
            var imageMap = A.Fake<Image>();

            A.CallTo(() => _recipeRepository.GetRecipe(recipeId)).Returns(recipe);
            A.CallTo(() => _mapper.Map<Image>(createImage)).Returns(imageMap);
            A.CallTo(() => _imageRepository.CreateImage(imageMap)).Returns(true);
            


            // Simulate authorization
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";
            var authUserId = "1"; // Assuming the user is authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);


            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };


            // Act
            var result = controller.CreateImage(createImage);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
            var okResult = (OkObjectResult)result;
            okResult.StatusCode.Should().Be(200);
            okResult.Value.Should().Be("Successfully created");
        }

        [Fact]
        public void ImageController_CreateImage_ReturnsBadRequest_WhenCreateImageIsNull()
        {
            // Arrange
            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.CreateImage(null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ImageController_CreateImage_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);
            controller.ModelState.AddModelError("key", "error message");

            // Act
            var result = controller.CreateImage(new ImageDto());

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ImageController_CreateImage_ReturnsUnprocessableEntity_WhenRecipeDoesNotExist()
        {
            // Arrange
            var createImage = new ImageDto { RecipeId = 999 }; // Assuming this recipe ID doesn't exist
            A.CallTo(() => _recipeRepository.GetRecipe(createImage.RecipeId)).Returns(null);

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.CreateImage(createImage);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>().Which.StatusCode.Should().Be(422);
        }



        [Fact]
        public void ImageController_UpdateImage_ReturnsBadRequest_WhenUpdateImageIsNull()
        {
            // Arrange
            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.UpdateImage(1, null);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ImageController_UpdateImage_ReturnsBadRequest_WhenImageIdMismatch()
        {
            // Arrange
            var imageId = 1;
            var updateImage = new ImageDto { Id = 2 }; // Assuming this image ID doesn't match imageId

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService);

            // Act
            var result = controller.UpdateImage(imageId, updateImage);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void ImageController_DeleteImage_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var imageId = 1; // Assuming this image ID doesn't exist

            A.CallTo(() => _imageRepository.ImageExists(imageId)).Returns(false);

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.DeleteImage(imageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void ImageController_DeleteImage_ReturnsForbid_WhenUserIsNotAuthorized()
        {
            // Arrange
            var imageId = 1; // Assuming this image ID exists
            var recipe = new Recipe { Id = 1, Name = "name", Instructions = "asd", User = new User { Id = 1, Username = "username", Password = "pasword", Email = "email", Name = "name" } };
            var deleteImage = new Image { Id = imageId, Recipe = recipe }; // User ID matches authUserId

            A.CallTo(() => _imageRepository.ImageExists(imageId)).Returns(true);
            A.CallTo(() => _imageRepository.GetImage(imageId)).Returns(deleteImage);
            A.CallTo(() => _recipeRepository.GetRecipe(deleteImage.Recipe.Id)).Returns(recipe);

            // Simulate authorization
            var authUserId = "2"; // Assuming the user is not authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.DeleteImage(imageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void ImageController_DeleteImage_ReturnsOk_WhenImageIsDeleted()
        {
            // Arrange
            var imageId = 1; // Assuming this image ID exists
            var recipe = new Recipe { Id = 1, Name = "name", Instructions = "asd", User = new User { Id = 1, Username = "username", Password = "pasword", Email = "email", Name = "name" } };
            var deleteImage = new Image { Id = imageId, Recipe = recipe }; // User ID matches authUserId

            A.CallTo(() => _imageRepository.ImageExists(imageId)).Returns(true);
            A.CallTo(() => _imageRepository.GetImage(imageId)).Returns(deleteImage);
            A.CallTo(() => _recipeRepository.GetRecipe(deleteImage.Recipe.Id)).Returns(recipe);
            A.CallTo(() => _imageRepository.DeleteImage(deleteImage)).Returns(true);

            // Simulate authorization
            var authUserId = "1"; // Assuming the user is authorized
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns(authUserId);

            var controller = new ImageController(_imageRepository, _mapper, _recipeRepository, _authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };

            // Act
            var result = controller.DeleteImage(imageId);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
