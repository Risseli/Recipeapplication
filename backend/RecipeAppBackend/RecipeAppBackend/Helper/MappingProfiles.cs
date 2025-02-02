﻿using AutoMapper;
using RecipeAppBackend.Dto;
using RecipeAppBackend.Models;

namespace RecipeAppBackend.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Ingredient, IngredientDto>().ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.Recipe.Id));
            CreateMap<User, UserDto>();
            CreateMap<Recipe, RecipeDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id));

            CreateMap<Review, ReviewDto>().ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User.Id))
                                          .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.Recipe.Id));
            CreateMap<Keyword, KeywordDto>();
            CreateMap<Image, ImageDto>().ForMember(dest => dest.ImageData, opt => opt.MapFrom
                                            (src => Convert.ToBase64String(src.ImageData)))
                                        .ForMember(dest => dest.RecipeId, opt => opt.MapFrom(src => src.Recipe.Id));
            



            CreateMap<Ingredient, IngredientDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<Recipe, RecipeDto>().ReverseMap();
            CreateMap<Review, ReviewDto>().ReverseMap();
            CreateMap<Keyword, KeywordDto>().ReverseMap();
            CreateMap<ImageDto, Image>().ForMember(dest => dest.ImageData, opt => opt.MapFrom
                                            (src => Convert.FromBase64String(src.ImageData)));
        }
    }
}
