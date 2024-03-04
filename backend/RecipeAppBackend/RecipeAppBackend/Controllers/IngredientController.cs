using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using System.Diagnostics;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IngredientController : Controller
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;

        public IngredientController(IIngredientRepository ingredientRepository
            , IMapper mapper
            , IRecipeRepository recipeRepository
            , IAuthService authService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _authService = authService;
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


        [Authorize]
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

            
            
            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (recipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            var ingredientMap = _mapper.Map<Ingredient>(ingredientCreate);
            ingredientMap.Recipe = recipe;

            if (!_ingredientRepository.CreateIngredient(ingredientMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the ingredient: " + ingredientMap.Id);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }


        [Authorize]
        [HttpPut("{ingId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult UpdateIngredient(int ingId, [FromBody] IngredientDto updateIngredient)
        {
            if (updateIngredient == null)
                return BadRequest(ModelState);
            
            if (updateIngredient.Id != 0 && ingId != updateIngredient.Id)
                return BadRequest(ModelState);

            var oldIngredient = _ingredientRepository.GetIngredient(ingId);

            if (oldIngredient == null)
                return NotFound();


            //Authorize user
            var authRecipe = _recipeRepository.GetRecipe(oldIngredient.Recipe.Id);
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (authRecipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            oldIngredient.Name = updateIngredient.Name != null ? updateIngredient.Name : oldIngredient.Name;
            oldIngredient.Amount = updateIngredient.Amount > 0 ? updateIngredient.Amount : oldIngredient.Amount;
            oldIngredient.Unit = updateIngredient.Unit != null ? updateIngredient.Unit : oldIngredient.Unit;

            if (updateIngredient.RecipeId != 0)
            {
                var recipe = _recipeRepository.GetRecipe(updateIngredient.RecipeId);

                if (recipe == null) //if no recipe exists
                {
                    ModelState.AddModelError("", "There is no recipe with the id: " + updateIngredient.RecipeId);
                    return StatusCode(422, ModelState);
                }

                oldIngredient.Recipe = recipe;
            }


            if (!_ingredientRepository.UpdateIngredient(oldIngredient))
            {
                ModelState.AddModelError("", "Something went wrong while updating ingredient: " + oldIngredient.Id);
                return StatusCode(500, ModelState);
            }


            return Ok("Succesfully updated");
        }


        [Authorize]
        [HttpDelete("{ingId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteIngredient(int ingId)
        {
            if (!_ingredientRepository.IngredientExists(ingId))
                return NotFound();

            var deleteIng = _ingredientRepository.GetIngredient(ingId);


            //Authorize user
            var recipe = _recipeRepository.GetRecipe(deleteIng.Recipe.Id);

            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (recipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();




            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_ingredientRepository.DeleteIngredient(deleteIng))
            {
                ModelState.AddModelError("", "Something went wrong while deleting ingredient: " + ingId);
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully deleted");
        }
    }
}
