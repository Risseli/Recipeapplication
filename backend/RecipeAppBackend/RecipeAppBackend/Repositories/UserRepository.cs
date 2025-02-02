﻿using Microsoft.EntityFrameworkCore;
using RecipeAppBackend.Data;
using RecipeAppBackend.Interfaces;
using RecipeAppBackend.Models;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace RecipeAppBackend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public bool AddFavorite(Favorite favorite)
        {
            _context.Favorites.Add(favorite);
            return Save();
        }

        public bool CreateUser(User user)
        {
            _context.Users.Add(user);
            return Save();
        }

        public bool DeleteUser(User user)
        {
            var reviews = GetUsersReviews(user.Id);
            var favorites = _context.Favorites.Where(f => f.UserId == user.Id).ToList();

            _context.RemoveRange(reviews);
            _context.RemoveRange(favorites);

            _context.Remove(user);

            return Save();
        }

        public bool FavoriteExists(int userId, int recipeId)
        {
            return _context.Favorites.Any(f => f.UserId == userId && f.RecipeId == recipeId);
        }

        public User GetUser(int id)
        {
            return _context.Users.Where(u => u.Id == id).FirstOrDefault();
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.Where(u => u.Email == email).FirstOrDefault();
        }

        public User GetUserByUsername(string username)
        {
            return _context.Users.Where(u => u.Username == username).FirstOrDefault();
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(u => u.Id).ToList();
        }

        public ICollection<Recipe> GetUsersFavorites(int id)
        {
            return _context.Favorites.Where(f => f.UserId == id).Select(f => f.Recipe).ToList();
        }

        public ICollection<Recipe> GetUsersRecipes(int id)
        {
            return _context.Recipes.Where(r => r.User.Id == id).ToList();
        }

        public ICollection<Review> GetUsersReviews(int id)
        {
            return _context.Reviews.Include(r => r.User).Include(r => r.Recipe).Where(r => r.User.Id == id).ToList();
        }

        public bool RemoveFavorite(Favorite favorite)
        {
            _context.Remove(favorite);
            return Save();
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateUser(User user)
        {
            _context.Update(user);
            return Save();
        }

        public bool UserExists(int id)
        {
            return _context.Users.Any(u => u.Id == id);
        }

        public bool ValidateEmail(string email)
        {
            string reg_pattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";

            Regex regex = new Regex(reg_pattern);

            return regex.IsMatch(email);
        }
    }
}
