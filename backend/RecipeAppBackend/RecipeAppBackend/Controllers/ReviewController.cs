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
        private readonly IMapper _mapper;
        private readonly IUserRepository userRepository;

        public ReviewController(IReviewRepository reviewRepository, IMapper mapper, IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            this.userRepository = userRepository;
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
    }
}
