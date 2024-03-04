using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using RecipeAppBackend.Services;
using System.Collections.Generic;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KeywordController : Controller
    {
        private readonly IKeywordRepository _keywordRepository;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public KeywordController(IKeywordRepository keywordRepository
            , IAuthService authService
            , IMapper mapper)
        {
            _keywordRepository = keywordRepository;
            _authService = authService;
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
            keywordMap.Word = keywordMap.Word.Trim().ToLower();

            if (!_keywordRepository.CreateKeyword(keywordMap))
            {
                ModelState.AddModelError("", "Something went wrong while creating the keyword " + keywordMap.Word);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }


        [Authorize]
        [HttpPut("{keywordId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public IActionResult UpdateKeyword(int keywordId, [FromBody] KeywordDto updateKeyword)
        {
            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var isAdmin = _authService.IsAdmin(token);

            if (!isAdmin)
                return Forbid();


            if (updateKeyword == null) 
                return BadRequest(ModelState);

            if (updateKeyword.Id != 0 && keywordId != updateKeyword.Id)
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var oldKeyword = _keywordRepository.GetKeyword(keywordId);

            if (oldKeyword == null)
                return NotFound();

            oldKeyword.Word = updateKeyword.Word != null ? updateKeyword.Word.Trim().ToLower() : oldKeyword.Word;

            if (!_keywordRepository.UpdateKeyword(oldKeyword))
            {
                ModelState.AddModelError("", "Something went wrong while updating keyword: " + keywordId);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully updated");
        }


        [Authorize]
        [HttpDelete("{keywordId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public IActionResult DeleteKeyword(int keywordId)
        {
            //Authorize user
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            var isAdmin = _authService.IsAdmin(token);

            if (!isAdmin)
                return Forbid();


            if (!_keywordRepository.KeywordExists(keywordId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var deleteKeyword = _keywordRepository.GetKeyword(keywordId);

            if (!_keywordRepository.DeleteKeyword(deleteKeyword))
            {
                ModelState.AddModelError("", "Something went wrong while deleting keyword: " + keywordId);
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully deleted");
        }
    }
}
