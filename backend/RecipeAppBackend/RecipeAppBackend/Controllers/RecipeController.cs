using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : Controller
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IKeywordRepository _keywordRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public RecipeController(IRecipeRepository recipeRepository
            , IUserRepository userRepository
            , IImageRepository imageRepository
            , IIngredientRepository ingredientRepository
            , IKeywordRepository keywordRepository
            , IAuthService authService
            , IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _ingredientRepository = ingredientRepository;
            _keywordRepository = keywordRepository;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Recipe>))]
        [ProducesResponseType(400)]
        public IActionResult GetRecipes()
        {
            var recipes = _mapper.Map<List<RecipeDto>>(_recipeRepository.GetRecipes());
            
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

        [HttpGet("{recipeId}")]
        [ProducesResponseType(200, Type = typeof(Recipe))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetRecipe(int recipeId)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            var recipe = _mapper.Map<RecipeDto>(_recipeRepository.GetRecipe(recipeId));

            recipe.Ingredients = _mapper.Map<List<IngredientDto>>(_recipeRepository.GetIngredientsOfRecipe(recipeId));
            recipe.Keywords = _mapper.Map<List<KeywordDto>>(_recipeRepository.GetKeywordsOfRecipe(recipeId));
            recipe.Reviews = _mapper.Map<List<ReviewDto>>(_recipeRepository.GetReviewsOfRecipe(recipeId));
            recipe.Images = _mapper.Map<List<ImageDto>>(_recipeRepository.GetImagesOfRecipe(recipeId));
            recipe.FavoriteCount = _recipeRepository.GetFavoriteCount(recipeId);
            recipe.Rating = _recipeRepository.GetRating(recipeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(recipe);
        }

        [HttpGet("{recipeId}/Ingredients")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Ingredient>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetIngredientsOfRecipe(int recipeId)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredients = _mapper.Map<List<IngredientDto>>(_recipeRepository.GetIngredientsOfRecipe(recipeId));

            return Ok(ingredients);
        }

        [HttpGet("{recipeId}/Images")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Image>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetImagesOfRecipe(int recipeId)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var images = _mapper.Map<List<ImageDto>>(_recipeRepository.GetImagesOfRecipe(recipeId));

            return Ok(images);
        }

        [HttpGet("{recipeId}/Keywords")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Keyword>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetKeywordsOfRecipe(int recipeId)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var keywords = _mapper.Map<List<KeywordDto>>(_recipeRepository.GetKeywordsOfRecipe(recipeId));

            return Ok(keywords);
        }

        [HttpGet("{recipeId}/Reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetReviewsOfRecipe(int recipeId)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var reviews = _mapper.Map<List<ReviewDto>>(_recipeRepository.GetReviewsOfRecipe(recipeId));

            return Ok(reviews);
        }


        [Authorize]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        public IActionResult CreateRecipe([FromBody] RecipeDto createRecipe)
        {
            if (createRecipe == null)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);

            if (createRecipe.UserId.ToString() != authUserId)
                return Forbid();



            //Handle user
            var user = _userRepository.GetUser(createRecipe.UserId);
            if (user == null)
            {
                ModelState.AddModelError("ModelStateError", "There is no user with the id: " +  createRecipe.UserId);
                return StatusCode(404, ModelState);
            }

            //Map the recipe
            var recipeMap = _mapper.Map<Recipe>(createRecipe); //map the recipe
            recipeMap.User = user;


            //Handle keywords
            var keywords = _mapper.Map<List<Keyword>>(createRecipe.Keywords);
            List<RecipeKeyword> recipeKeywords = new List<RecipeKeyword>();

            foreach (Keyword key in keywords)
            {
                var oldKeyword = _keywordRepository.GetKeywords()
                .Where(k => k.Word.Trim().ToLower() == key.Word.Trim().ToLower())
                .FirstOrDefault(); //search for same keyword

                if (oldKeyword == null) //if it doesnt exists, make it and add that one
                {
                    if (!_keywordRepository.CreateKeyword(key))
                    {
                        ModelState.AddModelError("ModelStateError", "Something went wrong while creating keyword: " + key.Word);
                        return StatusCode(500, ModelState);
                    }

                    recipeKeywords.Add(new RecipeKeyword
                    {
                        Recipe = recipeMap,
                        Keyword = key
                    });
                }
                else
                {       //if it exists, just add a connection to that one
                    recipeKeywords.Add(new RecipeKeyword
                    {
                        Recipe = recipeMap,
                        Keyword = oldKeyword
                    });
                }
                
            }

            //Create the recipe
            if (!_recipeRepository.CreateRecipe(recipeMap, recipeKeywords))
            {
                ModelState.AddModelError("ModelStateError", "Something went wrong while creating the recipe");
                return StatusCode(500, ModelState);
            }


            //Handle ingredients
            var ingredients = _mapper.Map<List<Ingredient>>(createRecipe.Ingredients);

            foreach(Ingredient ing in ingredients)
            {
                ing.Recipe = recipeMap;

                if (!_ingredientRepository.CreateIngredient(ing))
                {
                    ModelState.AddModelError("ModelStateError", "Something went wrong while creating ingredient: " + ing);
                    return StatusCode(500, ModelState);
                }
            }

            //Handle images
            var images = _mapper.Map<List<Image>>(createRecipe.Images);

            for (int i = 0; i < images.Count; i++)
            {
                images[i].Recipe = recipeMap;

                if (!_imageRepository.CreateImage(images[i]))
                {
                    ModelState.AddModelError("ModelStateError", "Something went wrong while creating image number: " + i);
                }
            }

            return Ok("Successfully created");
        }

        [Authorize]
        [HttpPost("{recipeId}/Keywords")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ProducesResponseType(404)]
        public IActionResult AddKeyword(int recipeId, [FromQuery] string keyword)
        {
            if (keyword.Length == 0)
                return BadRequest(ModelState);

            var recipe = _recipeRepository.GetRecipe(recipeId);

            if (recipe == null)
                return NotFound();


            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (recipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();


            RecipeKeyword recipeKeyword;

            //search for same keyword
            var oldKeyword = _keywordRepository.GetKeywords()
                .Where(k => k.Word.Trim().ToLower() == keyword.Trim().ToLower())
                .FirstOrDefault(); 

            if (oldKeyword == null) //if it doesnt exists, make it and add that one
            {
                Keyword key = new Keyword
                {
                    Word = keyword.Trim().ToLower()
                };

                if (!_keywordRepository.CreateKeyword(key))
                {
                    ModelState.AddModelError("ModelStateError", "Something went wrong while creating keyword: " + key.Word);
                    return StatusCode(500, ModelState);
                }

                //Make the connection
                recipeKeyword = new RecipeKeyword
                {
                    Recipe = recipe,
                    Keyword = key
                };
            }
            else
            {       //if it exists, just add a connection to that one

                //Checking for an existing connection
                if (_recipeRepository.KeywordExists(recipe.Id, oldKeyword.Id))
                {
                    ModelState.AddModelError("ModelStateError","The recipe already has the keyword '" +  oldKeyword.Word + "'");
                    return StatusCode(422, ModelState);
                }

                recipeKeyword = new RecipeKeyword
                {
                    Recipe = recipe,
                    Keyword = oldKeyword
                };
            }

            if (!_recipeRepository.AddKeyword(recipeKeyword))
            {
                ModelState.AddModelError("ModelStateError", "Something went wrong while creating the connection");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }



        [Authorize]
        [HttpPut("{recipeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult UpdateRecipe(int recipeId, [FromBody]RecipeDto updateRecipe)
        {
            if (updateRecipe == null)
                return BadRequest(ModelState);

            if (updateRecipe.Id != 0 && updateRecipe.Id != recipeId)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oldRecipe = _recipeRepository.GetRecipe(recipeId);
            if (oldRecipe == null)
                return NotFound();


            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (oldRecipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();


            oldRecipe.Name = updateRecipe.Name != null ? updateRecipe.Name : oldRecipe.Name;
            oldRecipe.Instructions = updateRecipe.Instructions != null ? updateRecipe.Instructions : oldRecipe.Instructions;
            oldRecipe.Visibility = updateRecipe.Visibility != null ? (bool)updateRecipe.Visibility : oldRecipe.Visibility;

            if (updateRecipe.UserId != 0)
            {
                var user = _userRepository.GetUser(updateRecipe.UserId);

                if (user == null)
                {
                    ModelState.AddModelError("ModelStateError","There is no user with the id: " + updateRecipe.UserId);
                    return StatusCode(422, ModelState);
                }

                oldRecipe.User = user;
            }

            if (!_recipeRepository.UpdateRecipe(oldRecipe))
            {
                ModelState.AddModelError("ModelStateError", "Something went wrong while updating recipe: " + recipeId);
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully updated");
        }

        [Authorize]
        [HttpDelete("{recipeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteRecipe(int recipeId)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            var deleteRecipe = _recipeRepository.GetRecipe(recipeId);



            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (deleteRecipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_recipeRepository.DeleteRecipe(deleteRecipe))
            {
                ModelState.AddModelError("ModelStateError", "Something went wrong while deleting recipe: " + recipeId);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully deleted");
        }

        [Authorize]
        [HttpDelete("{recipeId}/Keywords")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult RemoveKeyword(int recipeId, [FromQuery] string keyword)
        {
            if (!_recipeRepository.RecipeExists(recipeId))
                return NotFound();

            if (keyword.Length == 0)
                return BadRequest(ModelState);

            var recipe = _recipeRepository.GetRecipe(recipeId);

            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (recipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();


            var removeRecipeKeyword = _recipeRepository.GetRecipeKeywordsOfRecipe(recipeId)
                .Where(rk => rk.Keyword.Word.Trim().ToLower() == keyword.Trim().ToLower()).FirstOrDefault();

            if (removeRecipeKeyword == null)
            {
                ModelState.AddModelError("ModelStateError","The recipe " +  recipeId + " doesn't have the keyword '" + keyword + "'");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_recipeRepository.RemoveKeyword(removeRecipeKeyword))
            {
                ModelState.AddModelError("ModelStateError", "Something went wrong while removing keyword '" + keyword + "'");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully removed");
        }
    }
}
