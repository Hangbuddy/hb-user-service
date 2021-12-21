using System;
using AutoMapper;
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
            CreateMap<ApplicationUserCreateDto, ApplicationUser>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now));
            CreateMap<ApplicationUser, ApplicationUserReadDto>();
        }
    }
}