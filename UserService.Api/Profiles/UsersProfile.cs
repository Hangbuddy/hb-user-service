using System;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserService.Dtos.Requests;
using UserService.Dtos.Responses;
using UserService.Models;

namespace UserService.Profiles
{
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            // Source -> Target
            CreateMap<ApplicationUserRead, ApplicationUserReadDto>();
            CreateMap<ApplicationUserUpdateDto, ApplicationUser>()
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<IdentityUser, ApplicationUser>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));
        }
    }
}