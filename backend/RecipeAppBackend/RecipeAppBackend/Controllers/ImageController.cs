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
    public class ImageController : Controller
    {
        private readonly IImageRepository _imageRepository;
        private readonly IMapper _mapper;

        public ImageController(IImageRepository imageRepository, IMapper mapper)
        {
            _imageRepository = imageRepository;
            _mapper = mapper;
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

            var imageMap = _mapper.Map<Image>(createImage);
            //imageMap.ImageData = imageData;

            if (!_imageRepository.CreateImage(imageMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the image.");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }

    }
}
