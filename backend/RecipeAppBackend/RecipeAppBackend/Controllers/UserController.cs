﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using RecipeAppBackend.Repositories;
using System.Collections.Generic;

namespace RecipeAppBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, IRecipeRepository recipeRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type =  typeof(IEnumerable<User>))]
        public IActionResult GetUsers()
        {
            var users = _mapper.Map<List<UserDto>>(_userRepository.GetUsers());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(users);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(200, Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUser(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var user = _mapper.Map<UserDto>(_userRepository.GetUser(userId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpGet("{userId}/Favorites")]
        [ProducesResponseType(200, Type=typeof(IEnumerable<Recipe>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersFavorites(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var recipes = _mapper.Map<List<RecipeDto>>(_userRepository.GetUsersFavorites(userId));

            if (recipes.Count == 0)
            {
                ModelState.AddModelError("", "User has no favorites.");
                return BadRequest(ModelState);
            }

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

        [HttpGet("{userId}/Recipes")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Recipe>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersRecipes(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var recipes = _mapper.Map<List<RecipeDto>>(_userRepository.GetUsersRecipes(userId));

            if (recipes.Count == 0)
            {
                ModelState.AddModelError("", "User has no recipes.");
                return BadRequest(ModelState);
            }

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

        [HttpGet("{userId}/Reviews")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Review>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult GetUsersReviews(int userId)
        {
            if (!_userRepository.UserExists(userId))
                return NotFound();

            var reviews = _mapper.Map<List<ReviewDto>>(_userRepository.GetUsersReviews(userId));

            if (reviews.Count == 0)
            {
                ModelState.AddModelError("", "User has no reviews.");
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(reviews);
        }
    }
}
