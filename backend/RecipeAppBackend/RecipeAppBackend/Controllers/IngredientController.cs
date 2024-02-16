using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : Controller
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepository;

        public IngredientController(IIngredientRepository ingredientRepository
            , IMapper mapper
            , IRecipeRepository recipeRepository)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _recipeRepository = recipeRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Ingredient>))]
        public IActionResult GetIngredients()
        {
            var ingredients = _mapper.Map<List<IngredientDto>>(_ingredientRepository.GetIngredients());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(ingredients);
        }

        [HttpGet("{ingId}")]
        [ProducesResponseType(200, Type = typeof(Ingredient))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetIngredient(int ingId)
        {
            if (!_ingredientRepository.IngredientExists(ingId))
                return NotFound();

            var ingredient = _mapper.Map<IngredientDto>(_ingredientRepository.GetIngredient(ingId));

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(ingredient);
        }


        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult CreateIngredient([FromBody] IngredientDto ingredientCreate)
        {
            if (ingredientCreate == null)
                return BadRequest(ModelState);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int recipeId = ingredientCreate.RecipeId;
            var recipe = _recipeRepository.GetRecipe(recipeId);

            if (recipe == null)
            {
                ModelState.AddModelError("", "There is no recipe with the id: " + recipeId);
                return StatusCode(422, ModelState);
            }

            var ingredientMap = _mapper.Map<Ingredient>(ingredientCreate);
            ingredientMap.Recipe = recipe;

            if (!_ingredientRepository.CreateIngredient(ingredientMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the ingredient" + ingredientMap.Name);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }
    }
}
