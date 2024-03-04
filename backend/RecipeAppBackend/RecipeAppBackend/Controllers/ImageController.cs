using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using RecipeAppBackend.Services;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : Controller
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IAuthService _authService;

        public ImageController(IImageRepository imageRepository
            , IMapper mapper
            , IRecipeRepository recipeRepository
            , IAuthService authService)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
            _recipeRepository = recipeRepository;
            _authService = authService;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Image>))]
        public IActionResult GetImages()
        {
            var images = _mapper.Map<List<ImageDto>>(_imageRepository.GetImages());

            //foreach (var image in images)
            //{
            //    var base64data = Convert.ToBase64String(image.ImageData);
            //    image.ImageData = base64data;
            //}

            if (!ModelState.IsValid) 
                return BadRequest(ModelState);

            return Ok(images);
        }

        [HttpGet("{imageId}")]
        [ProducesResponseType(200, Type = typeof(Image))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetImage(int imageId)
        {
            if (!_imageRepository.ImageExists(imageId))
                return NotFound();

            var image = _mapper.Map<ImageDto>(_imageRepository.GetImage(imageId));

            return Ok(image);
        }



        [Authorize]
        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult CreateImage([FromBody] ImageDto createImage)
        {
            if (createImage == null)
                return BadRequest(ModelState);

            //This isn't needed, since the automapper mapping profile takes care of it. Or something else does
            //byte[] imageData = Convert.FromBase64String(createImage.ImageData);

            //if (imageData == null)
            //{
            //    ModelState.AddModelError("", "There was an issue converting image data");
            //    return BadRequest(ModelState);
            //}

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int recipeId = createImage.RecipeId;
            var recipe = _recipeRepository.GetRecipe(recipeId);

            if (recipe == null)
            {
                ModelState.AddModelError("", "There is no recipe with the Id: " + recipeId);
                return StatusCode(422, ModelState);
            }



            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);

            if (recipe.User.Id.ToString() != authUserId)
                return Forbid();



            var imageMap = _mapper.Map<Image>(createImage);
            //imageMap.ImageData = imageData;
            imageMap.Recipe = recipe;

            if (!_imageRepository.CreateImage(imageMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the image.");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");
        }


        [Authorize]
        [HttpPut("{imageId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult UpdateImage(int imageId, [FromBody] ImageDto updateImage)
        {
            if (updateImage == null)
                return BadRequest(ModelState);

            if (updateImage.Id != 0 && updateImage.Id != imageId)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oldImage = _imageRepository.GetImage(imageId);
            if (oldImage == null)
                return NotFound();


            //Authorize user
            var authRecipe = _recipeRepository.GetRecipe(oldImage.Recipe.Id);
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (authRecipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            //Set the new recipe id
            if (updateImage.RecipeId != 0)
            {
                var recipe = _recipeRepository.GetRecipe(updateImage.RecipeId);

                if (recipe == null)
                {
                    ModelState.AddModelError("", "There is no recipe with the id: " + updateImage.RecipeId);
                    return StatusCode(422, ModelState);
                }

                oldImage.Recipe = recipe;
            }

            //Set the new imagedata
            var updateImageMap = _mapper.Map<Image>(updateImage);

            oldImage.ImageData = updateImageMap.ImageData.Length > 0 ? updateImageMap.ImageData : oldImage.ImageData;

            if (!_imageRepository.UpdateImage(oldImage))
            {
                ModelState.AddModelError("", "Something went wrong while updating image: " + oldImage.Id);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully updated");
        }



        [Authorize]
        [HttpDelete("{imageId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteImage(int imageId)
        {
            if (!_imageRepository.ImageExists(imageId))
                return NotFound();

            var deleteImage = _imageRepository.GetImage(imageId);


            //Authorize user
            var recipe = _recipeRepository.GetRecipe(deleteImage.Recipe.Id);
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var authUserId = _authService.GetUserId(token);
            var isAdmin = _authService.IsAdmin(token);

            if (recipe.User.Id.ToString() != authUserId && !isAdmin)
                return Forbid();



            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_imageRepository.DeleteImage(deleteImage))
            {
                ModelState.AddModelError("","Something went wrong while deleting image: " + imageId);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully deleted");
        }
    }
}
