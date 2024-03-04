using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : Controller
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;

        public ReviewController(IReviewRepository reviewRepository
            , IMapper mapper
            , IUserRepository userRepository
            , IRecipeRepository recipeRepository
            , IAuthService authService)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _authService = authService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        public IActionResult GetReviews()
        {
            var reviews = _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }

        [HttpGet("{reviewId}")]
        [ProducesResponseType(200, Type = typeof(Keyword))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetKeyword(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            var review = _mapper.Map<ReviewDto>(_reviewRepository.GetReview(reviewId));

            //var revieww = _reviewRepository.GetReview(reviewId);
            
            //revieww.User = userRepository.GetUser(3);

            //var review = _mapper.Map<ReviewDto>(revieww);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(review);
        }


        [Authorize]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult CreateReview([FromBody] ReviewDto createReview)
        {
            if (createReview == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            

            if (createReview.UserId.ToString() != authUserId)
                return Forbid();



            //Limit rating to 1-5
            if (createReview.Rating <= 1) createReview.Rating = 1;
            if (createReview.Rating >= 5) createReview.Rating = 5;

            //retrieving the user
            var userId = createReview.UserId;
            var user = _userRepository.GetUser(userId); 

            if (user == null)   //Checking that the user existed
            {
                ModelState.AddModelError("","There is no user with the Id: " + userId);
                return StatusCode(422, ModelState);
            }


            //retrieving the recipe
            var recipeId = createReview.RecipeId;
            var recipe = _recipeRepository.GetRecipe(recipeId); 

            if (recipe == null)   //Checking that the recipe existed
            {
                ModelState.AddModelError("", "There is no recipe with the Id: " + recipeId);
                return StatusCode(422, ModelState);
            }

            var reviewMap = _mapper.Map<Review>(createReview);

            reviewMap.User = user;
            reviewMap.Recipe = recipe;

            if (!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the review.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }


        [Authorize]
        [HttpPut("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult UpdateReview(int reviewId, [FromBody]ReviewDto updateReview)
        {
            if (updateReview == null)
                return BadRequest(ModelState);

            if (updateReview.Id != 0 && updateReview.Id != reviewId)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oldReview = _reviewRepository.GetReview(reviewId);

            if (oldReview == null)
                return NotFound();


            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (oldReview.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            //Rating
            if (updateReview.Rating != 0)
            {
                int rating = updateReview.Rating;
                if (rating <= 1) rating = 1;
                else if (rating >= 5) rating = 5;

                oldReview.Rating = rating;
            }

            //Comment
            oldReview.Comment = updateReview.Comment != null ? updateReview.Comment : oldReview.Comment;

            //User
            if (updateReview.UserId != 0)
            {
                var user = _userRepository.GetUser(updateReview.UserId);

                if (user == null)
                {
                    ModelState.AddModelError("", "There is no user with the id: " + updateReview.UserId);
                    return StatusCode(422, ModelState);
                } 
                    
                oldReview.User = user;
            }
           
            
            //Recipe
            if (updateReview.RecipeId != 0)
            {
                var recipe = _recipeRepository.GetRecipe(updateReview.RecipeId);

                if (recipe == null)
                {
                    ModelState.AddModelError("", "There is no recipe with the id: " + updateReview.RecipeId);
                    return StatusCode(422, ModelState);
                }

                oldReview.Recipe = recipe;
            }
            

            //Update
            if (!_reviewRepository.UpdateReview(oldReview))
            {
                ModelState.AddModelError("", "Something went wrong while updating review: " + reviewId);
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }


        [Authorize]
        [HttpDelete("{reviewId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExists(reviewId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deleteReview = _reviewRepository.GetReview(reviewId);


            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (deleteReview.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            if (!_reviewRepository.DeleteReview(deleteReview))
            {
                ModelState.AddModelError("", "Something went wrong while deleting review: " + reviewId);
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
