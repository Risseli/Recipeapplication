using FakeItEasy;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeAppBackend.Services;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Controllers;
using FluentAssertions;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Tests.Controller
{
    public class ReviewControllerTests
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;
        private readonly DefaultHttpContext _httpContext;

        public ReviewControllerTests()
        {
            _reviewRepository = A.Fake<IReviewRepository>();
            _mapper = A.Fake<IMapper>();
            _userRepository = A.Fake<IUserRepository>();
            _recipeRepository = A.Fake<IRecipeRepository>();
            _authService = A.Fake<IAuthService>();
            _httpContext = new DefaultHttpContext();
        }

        private ReviewController GetReviewControllerInstanceWithAdminAuthorization()
        {
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(true);
            A.CallTo(() => authService.GetUserId(A<string>._)).Returns("1");

            return new ReviewController(_reviewRepository, _mapper, _userRepository, _recipeRepository, authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }

        private ReviewController GetReviewControllerInstanceWithoutAdminAuthorization()
        {
            var _httpContext = new DefaultHttpContext();
            _httpContext.Request.Headers["Authorization"] = "Bearer your_token_here";

            var authService = A.Fake<IAuthService>();
            A.CallTo(() => authService.IsAdmin(A<string>._)).Returns(false);

            return new ReviewController(_reviewRepository, _mapper, _userRepository, _recipeRepository, authService)
            {
                ControllerContext = new ControllerContext { HttpContext = _httpContext }
            };
        }


        [Fact]
        public void ReviewController_GetReviews_ReturnsOK_WithListOfReviews()
        {
            // Arrange
            var fakeReviews = new List<Review>
            {
                new Review { Id = 1, Rating = 4, Comment = "Great recipe"},
                new Review { Id = 2, Rating = 5, Comment = "Amazing dish"}
            };
            var fakeReviewsDto = new List<ReviewDto>
            {
                new ReviewDto { Id = 1, Rating = 4, Comment = "Great recipe", UserId = 1, RecipeId = 1 },
                new ReviewDto { Id = 2, Rating = 5, Comment = "Amazing dish", UserId = 2, RecipeId = 1 }
            };

            A.CallTo(() => _reviewRepository.GetReviews()).Returns(fakeReviews);
            A.CallTo(() => _mapper.Map<List<ReviewDto>>(fakeReviews)).Returns(fakeReviewsDto);

            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.GetReviews();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var reviews = okResult.Value.Should().BeAssignableTo<List<ReviewDto>>().Subject;

            reviews.Should().HaveCount(fakeReviewsDto.Count);
            reviews.Should().ContainEquivalentOf(fakeReviewsDto[0]); // Assuming ContainEquivalentOf checks equality by Id
            reviews.Should().ContainEquivalentOf(fakeReviewsDto[1]);
        }

        [Fact]
        public void ReviewController_GetReviews_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var controller = GetReviewControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error message");

            // Act
            var result = controller.GetReviews();

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        public void ReviewController_GetReview_ReturnsOk_WithReviewDto_WhenReviewExists()
        {
            // Arrange
            var fakeReview = new Review { Id = 1, Rating = 4, Comment = "Great recipe" };
            var fakeReviewDto = new ReviewDto { Id = 1, Rating = 4, Comment = "Great recipe", UserId = 1, RecipeId = 1 };

            A.CallTo(() => _reviewRepository.ReviewExists(fakeReview.Id)).Returns(true);
            A.CallTo(() => _reviewRepository.GetReview(fakeReview.Id)).Returns(fakeReview);
            A.CallTo(() => _mapper.Map<ReviewDto>(fakeReview)).Returns(fakeReviewDto);

            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.GetReview(fakeReview.Id);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(fakeReviewDto);
        }

        [Fact]
        public void ReviewController_GetReview_ReturnsNotFound_WhenReviewDoesNotExist()
        {
            // Arrange
            var fakeReviewId = 1;

            A.CallTo(() => _reviewRepository.ReviewExists(fakeReviewId)).Returns(false);

            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.GetReview(fakeReviewId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void ReviewController_GetReview_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fakeReviewId = 1;
            var controller = GetReviewControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error message");
                A.CallTo(() => _reviewRepository.ReviewExists(fakeReviewId)).Returns(true);

            // Act
            var result = controller.GetReview(fakeReviewId);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }


        [Fact]
        public void ReviewController_CreateReview_ReturnsOk_WhenReviewCreatedSuccessfully()
        {
            // Arrange
            var fakeReviewDto = new ReviewDto { Rating = 4, Comment = "Great recipe", UserId = 1, RecipeId = 1 };
            var fakeUser = new User { Id = 1 };
            var fakeRecipe = new Recipe { Id = 1 };

            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns("1");
            A.CallTo(() => _userRepository.GetUser(fakeReviewDto.UserId)).Returns(fakeUser);
            A.CallTo(() => _recipeRepository.GetRecipe(fakeReviewDto.RecipeId)).Returns(fakeRecipe);
            A.CallTo(() => _reviewRepository.CreateReview(A<Review>._)).Returns(true);

            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.CreateReview(fakeReviewDto);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().Be("Successfully created");
        }

        [Fact]
        public void ReviewController_CreateReview_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var fakeReviewDto = new ReviewDto();
            var controller = GetReviewControllerInstanceWithAdminAuthorization();
            controller.ModelState.AddModelError("Key", "Error message");

            // Act
            var result = controller.CreateReview(fakeReviewDto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ReviewController_CreateReview_ReturnsForbid_WhenUserIdDoesNotMatchAuthenticatedUser()
        {
            // Arrange
            var fakeReviewDto = new ReviewDto { UserId = 1 };
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns("2");

            var controller = GetReviewControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.CreateReview(fakeReviewDto);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }


        [Fact]
        public void ReviewController_UpdateReview_ReturnsBadRequest_WhenUpdateReviewIsNull()
        {
            // Arrange
            int reviewId = 1;
            ReviewDto updateReview = null;
            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateReview(reviewId, updateReview);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ReviewController_UpdateReview_ReturnsBadRequest_WhenReviewIdMismatch()
        {
            // Arrange
            int reviewId = 1;
            var updateReview = new ReviewDto { Id = 2 };
            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateReview(reviewId, updateReview);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public void ReviewController_UpdateReview_ReturnsNotFound_WhenReviewNotFound()
        {
            // Arrange
            int reviewId = 1;
            var updateReview = new ReviewDto { Id = reviewId };
            A.CallTo(() => _reviewRepository.GetReview(reviewId)).Returns(null);
            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.UpdateReview(reviewId, updateReview);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void ReviewController_UpdateReview_ReturnsForbid_WhenUnauthorized()
        {
            // Arrange
            int reviewId = 1;
            var oldReview = new Review { Id = reviewId, User = new User { Id = 2 } };
            var updateReview = new ReviewDto { Id = reviewId };
            A.CallTo(() => _reviewRepository.GetReview(reviewId)).Returns(oldReview);
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns("1");
            var controller = GetReviewControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.UpdateReview(reviewId, updateReview);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }


        [Fact]
        public void ReviewController_DeleteReview_ReturnsNotFound_WhenReviewNotFound()
        {
            // Arrange
            int reviewId = 1;
            A.CallTo(() => _reviewRepository.ReviewExists(reviewId)).Returns(false);
            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteReview(reviewId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void ReviewController_DeleteReview_ReturnsForbidden_WhenUnauthorized()
        {
            // Arrange
            int reviewId = 1;
            var deleteReview = new Review { Id = reviewId, User = new User { Id = 2 } };
            A.CallTo(() => _reviewRepository.ReviewExists(reviewId)).Returns(true);
            A.CallTo(() => _reviewRepository.GetReview(reviewId)).Returns(deleteReview);
            A.CallTo(() => _authService.GetUserId(A<string>._)).Returns("1");
            var controller = GetReviewControllerInstanceWithoutAdminAuthorization();

            // Act
            var result = controller.DeleteReview(reviewId);

            // Assert
            result.Should().BeOfType<ForbidResult>();
        }

        [Fact]
        public void ReviewController_DeleteReview_ReturnsOk_WhenReviewDeletedSuccessfully()
        {
            // Arrange
            int reviewId = 1;
            var deleteReview = new Review { Id = reviewId, User = new User { Id = 1 } };
            A.CallTo(() => _reviewRepository.ReviewExists(reviewId)).Returns(true);
            A.CallTo(() => _reviewRepository.GetReview(reviewId)).Returns(deleteReview);
            A.CallTo(() => _reviewRepository.DeleteReview(deleteReview)).Returns(true);
            var controller = GetReviewControllerInstanceWithAdminAuthorization();

            // Act
            var result = controller.DeleteReview(reviewId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }
    }
}
