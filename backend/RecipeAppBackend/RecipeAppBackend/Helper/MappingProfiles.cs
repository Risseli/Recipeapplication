using AutoMapper;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<User, UserDto>();
            CreateMap<Recipe, RecipeDto>();
            CreateMap<Review, ReviewDto>();
        }
    }
}
