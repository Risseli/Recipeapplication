using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        private readonly IMapper _mapper;

        public RecipeController(IRecipeRepository recipeRepository
            , IUserRepository userRepository
            , IImageRepository imageRepository
            , IIngredientRepository ingredientRepository
            , IKeywordRepository keywordRepository
            , IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _ingredientRepository = ingredientRepository;
            _keywordRepository = keywordRepository;
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

            //Getting and checking user
            var user = _userRepository.GetUser(createRecipe.UserId);
            if (user == null)
            {
                ModelState.AddModelError("","There is no user with the id: " +  createRecipe.UserId);
                return StatusCode(404, ModelState);
            }

            var recipeMap = _mapper.Map<Recipe>(createRecipe); //map the recipe
            recipeMap.User = user;

            //Handle ingredients
            var ingredients = _mapper.Map<List<Ingredient>>(createRecipe.Ingredients);
            List<RecipeIngredient> recipeIngredients = new List<RecipeIngredient>();

            foreach(Ingredient ing in ingredients)
            {
                if (_ingredientRepository.GetIngredients()
                    .Where(i => i.Name.Trim().ToLower() == ing.Name.Trim().ToLower())
                    .FirstOrDefault() == null) //doenst exist, make it
                {

                    //yhyy siinä muuttuu ne määrät ei näin voi tehdä!!!!!
                    //yhyy siinä muuttuu ne määrät ei näin voi tehdä!!!!!
                    //yhyy siinä muuttuu ne määrät ei näin voi tehdä!!!!!
                    //yhyy siinä muuttuu ne määrät ei näin voi tehdä!!!!!
                    //yhyy siinä muuttuu ne määrät ei näin voi tehdä!!!!!





                    if (!_ingredientRepository.CreateIngredient(ing))
                    {
                        ModelState.AddModelError("", "Something went wrong while creating ingredient: " + ing);
                        return StatusCode(500, ModelState);
                    }
                }

                recipeIngredients.Add(new RecipeIngredient
                {
                    Recipe = recipeMap,
                    Ingredient = ing
                });
            }

            //Handle keywords
            var keywords = _mapper.Map<List<Keyword>>(createRecipe.Keywords);
            List<RecipeKeyword> recipeKeywords = new List<RecipeKeyword>();

            return Ok("ei ole vielä valmis");
        }
    }
}
