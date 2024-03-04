using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;


namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository
            , IRecipeRepository recipeRepository
            , IAuthService authService
            , IMapper mapper)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _authService = authService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(200, Type =  typeof(IEnumerable<User>))]
        public IActionResult GetUsers()
        {

            //var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            //var userId = _authService.GetUserId(token);
            //var isAdmin = _authService.IsAdmin(token);

            //return Ok(userId + " " +  isAdmin);


            var users = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(users);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _mapper.Map<UserDto>(_userRepository.GetUser(userId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpGet("{userId}/Favorites")]
        [ProducesResponseType(200, Type=typeof(IEnumerable<Recipe>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersFavorites(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var recipes = _mapper.Map<List<RecipeDto>>(_userRepository.GetUsersFavorites(userId));

            //if (recipes.Count == 0)
            //{
            //    ModelState.AddModelError("", "User has no favorites.");
            //    return BadRequest(ModelState);
            //}

            foreach (var recipe in recipes)
            {
                recipe.Ingredients = _mapper.Map<List<IngredientDto>>(_recipeRepository.GetIngredientsOfRecipe(recipe.Id));
                recipe.Keywords = _mapper.Map<List<KeywordDto>>(_recipeRepository.GetKeywordsOfRecipe(recipe.Id));
                recipe.Reviews = _mapper.Map<List<ReviewDto>>(_recipeRepository.GetReviewsOfRecipe(recipe.Id));
                recipe.Images = _mapper.Map<List<ImageDto>>(_recipeRepository.GetImagesOfRecipe(recipe.Id));
                recipe.FavoriteCount = _recipeRepository.GetFavoriteCount(recipe.Id);
                recipe.Rating = _recipeRepository.GetRating(recipe.Id);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(recipes);
        }

        [HttpGet("{userId}/Recipes")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Recipe>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersRecipes(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var recipes = _mapper.Map<List<RecipeDto>>(_userRepository.GetUsersRecipes(userId));

            //if (recipes.Count == 0)
            //{
            //    ModelState.AddModelError("", "User has no recipes.");
            //    return BadRequest(ModelState);
            //}

            foreach (var recipe in recipes)
            {
                recipe.Ingredients = _mapper.Map<List<IngredientDto>>(_recipeRepository.GetIngredientsOfRecipe(recipe.Id));
                recipe.Keywords = _mapper.Map<List<KeywordDto>>(_recipeRepository.GetKeywordsOfRecipe(recipe.Id));
                recipe.Reviews = _mapper.Map<List<ReviewDto>>(_recipeRepository.GetReviewsOfRecipe(recipe.Id));
                recipe.Images = _mapper.Map<List<ImageDto>>(_recipeRepository.GetImagesOfRecipe(recipe.Id));
                recipe.FavoriteCount = _recipeRepository.GetFavoriteCount(recipe.Id);
                recipe.Rating = _recipeRepository.GetRating(recipe.Id);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(recipes);
        }

        [HttpGet("{userId}/Reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersReviews(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDto>>(_userRepository.GetUsersReviews(userId));

            //if (reviews.Count == 0)
            //{
            //    ModelState.AddModelError("", "User has no reviews.");
            //    return BadRequest(ModelState);
            //}

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }



        [HttpPost("register")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult CreateUser([FromBody] CreateUserDto createUser)
        {
            if (createUser == null)
                return BadRequest(ModelState);

            var user = _userRepository.GetUsers()
                .Where(u => u.Email == createUser.Email).FirstOrDefault();

            if (user != null)
            {
                ModelState.AddModelError("", "The email" + createUser.Email + " is already in use");
                return StatusCode(422, ModelState);
            }

            user = _userRepository.GetUsers()
                .Where(u => u.Username == createUser.Username).FirstOrDefault();

            if (user != null)
            {
                ModelState.AddModelError("", "The username" + createUser.Username + " is already in use");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Uusi käyttäjä ei ole koskaan admin
            createUser.Admin = false;

            //Hash the password
            createUser.Password = _userRepository.HashPassword(createUser.Password);

            var userMap = _mapper.Map<User>(createUser);

            if (!_userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the user");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }


        [HttpPost("login")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult Login([FromBody] LoginUserDto loginUser)
        {
            if (loginUser == null)
                return BadRequest(ModelState);

            var user = _userRepository.GetUsers()
                .Where(u => u.Username == loginUser.Username).FirstOrDefault();

            if (user == null || !_authService.VerifyPassword(loginUser.Password, user.Password))
            {
                ModelState.AddModelError("", "Incorrect username or password");
                return StatusCode(422, ModelState);
            }


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = _authService.GenerateToken(user);


            return Ok(new
            {
                UserId = user.Id,
                Token = token
            });
        }

        [HttpPost("{userId}/Favorites")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        public IActionResult AddFavorite(int userId, [FromQuery] int recipeId)
        {
            var user = _userRepository.GetUser(userId);
            var recipe = _recipeRepository.GetRecipe(recipeId);

            if (user == null)
            {
                ModelState.AddModelError("", "There is no user with the id: " + userId);
                return StatusCode(404, ModelState);
            }
            if (recipe == null)
            {
                ModelState.AddModelError("", "There is no recipe with the id: " + recipeId);
                return StatusCode(404, ModelState);
            }

            if (_userRepository.FavoriteExists(userId, recipeId))
            {
                ModelState.AddModelError("", "The recipe is already a favorite of the user");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Favorite favorite = new Favorite
            {
                UserId = userId,
                RecipeId = recipeId,
                Recipe = recipe,
                User = user
            };

            if (!_userRepository.AddFavorite(favorite))
            {
                ModelState.AddModelError("", "Something went wrong while adding the favorite");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }


        [HttpPut("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult UpdateUser(int userId, [FromBody] CreateUserDto updateUser)
        {
            if (updateUser == null) 
                return BadRequest(ModelState);

            if (updateUser.Id != 0 && updateUser.Id != userId)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oldUser = _userRepository.GetUser(userId);
            if (oldUser == null)
                return NotFound();

            oldUser.Admin = updateUser.Admin != null ? (bool)updateUser.Admin : oldUser.Admin;
            oldUser.Password = updateUser.Password != null ? _userRepository.HashPassword(updateUser.Password) : oldUser.Password;
            oldUser.Name = updateUser.Name != null ? updateUser.Name : oldUser.Name;


            //Username
            if (updateUser.Username != null)
            {
                var user = _userRepository.GetUsers()
                            .Where(u => u.Username == updateUser.Username).FirstOrDefault();

                if (user != null)
                {
                    ModelState.AddModelError("", "The username " + user.Username + " is already in use");
                    return StatusCode(422, ModelState);
                }

                oldUser.Username = updateUser.Username;
            }
           

            //Email
            if (updateUser.Email != null)
            {
                var user = _userRepository.GetUsers()
                    .Where(u => u.Email == updateUser.Email).FirstOrDefault();

                if (user != null )
                {
                    ModelState.AddModelError("", "The email " + user.Email + " is already in use");
                    return StatusCode(422, ModelState);
                }

                oldUser.Email = updateUser.Email;
            }
            
            if (!_userRepository.UpdateUser(oldUser))
            {
                ModelState.AddModelError("", "Something went wrong while updating user: " + userId);
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }


        [HttpDelete("{userId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Delete recipes
            var recipes = _userRepository.GetUsersRecipes(userId);

            foreach (var recipe in recipes)
            {
                if (!_recipeRepository.DeleteRecipe(recipe))
                {
                    ModelState.AddModelError("", "Something went wrong while deleting recipe: " + recipe.Id);
                    return StatusCode(500, ModelState);
                }
            }

            var deleteUser = _userRepository.GetUser(userId);

            if (!_userRepository.DeleteUser(deleteUser))
            {
                ModelState.AddModelError("", "Something went wrong while deleting user: " +  userId);
                return StatusCode(500, ModelState);
            }


            return Ok("Succesfully deleted");
        }


        [HttpDelete("{userId}/Favorites")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        public IActionResult RemoveFavorite(int userId, [FromQuery] int recipeId)
        {
            var user = _userRepository.GetUser(userId);
            var recipe = _recipeRepository.GetRecipe(recipeId);

            if (user == null)
            {
                ModelState.AddModelError("", "There is no user with the id: " + userId);
                return StatusCode(404, ModelState);
            }
            if (recipe == null)
            {
                ModelState.AddModelError("", "There is no recipe with the id: " + recipeId);
                return StatusCode(404, ModelState);
            }

            if (!_userRepository.FavoriteExists(userId, recipeId))
            {
                ModelState.AddModelError("", "The recipe is not a favorite of the user");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Favorite favorite = new Favorite
            {
                UserId = userId,
                RecipeId = recipeId,
                Recipe = recipe,
                User = user
            };

            if (!_userRepository.RemoveFavorite(favorite))
            {
                ModelState.AddModelError("", "Something went wrong while removing the favorite");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully removed");
        }
    }
}
