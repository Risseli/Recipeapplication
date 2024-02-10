using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using System.Collections.Generic;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : Controller
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IMapper _mapper;

        public KeywordController(IKeywordRepository keywordRepository, IMapper mapper)
        {
            _keywordRepository = keywordRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Keyword>))]
        public IActionResult GetKeywords()
        {
            var keywords = _mapper.Map<List<KeywordDto>>(_keywordRepository.GetKeywords());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(keywords);
        }

        [HttpGet("{keywordId}")]
        [ProducesResponseType(200, Type = typeof(Keyword))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetKeyword(int keywordId)
        {
            if (!_keywordRepository.KeywordExists(keywordId))
                return NotFound();

            var keyword = _mapper.Map<KeywordDto>(_keywordRepository.GetKeyword(keywordId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(keyword);
        }

        [HttpGet("recipe/{recipeId}")]
        [ProducesResponseType(200, Type = typeof(Keyword))]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        public IActionResult GetKeywordsOfRepice(int recipeId)
        {
            var keywords = _mapper.Map<List<KeywordDto>>(_keywordRepository.GetKeywordsOfRecipe(recipeId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(keywords);
        }



        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        public IActionResult CreateKeyword([FromBody] KeywordDto createKeyword)
        {
            if (createKeyword == null)
                return BadRequest(ModelState);

            var keyword = _keywordRepository.GetKeywords()
                .Where(k => k.Word.Trim().ToLower() == createKeyword.Word.Trim().ToLower())
                .FirstOrDefault();

            if (keyword != null)
            {
                ModelState.AddModelError("", "Keyword already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var keywordMap = _mapper.Map<Keyword>(createKeyword);

            if (!_keywordRepository.CreateKeyword(keywordMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the keyword " + keywordMap.Word);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }
    }
}
