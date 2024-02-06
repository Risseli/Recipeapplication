using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;

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
    }
}
