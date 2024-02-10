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

        public IngredientController(IIngredientRepository ingredientRepository, IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
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

            var ingredient = _ingredientRepository.GetIngredients()
                .Where(i => i.Name.Trim().ToLower() == ingredientCreate.Name.Trim().ToLower())
                .FirstOrDefault();

            if (ingredient != null)
            {
                ModelState.AddModelError("", "Ingredient already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredientMap = _mapper.Map<Ingredient>(ingredientCreate);

            if (!_ingredientRepository.CreateIngredient(ingredientMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the ingredient" + ingredientMap.Name);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }
    }
}
