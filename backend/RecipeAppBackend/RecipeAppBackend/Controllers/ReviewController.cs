using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public ReviewController(IReviewRepository reviewRepository, 
            IUserRepository userRepository,
            IRecipeRepository recipeRepository,
            IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
            _userRepository = userRepository;
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
    }
}
