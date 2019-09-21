using AutoMapper;
using DatingApp.Api.Extension;
using DatingApp.Api.Models;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using System.Collections.Generic;
using System.Linq;

namespace DatingApp.Api.Dtos
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Add as many of these lines as you need to map your objects
            CreateMap<Users, UserForDetailedDto>()
                .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(d => d.Age, opt => opt.MapFrom(d => d.DateOfBirth.CalculateAge()));

            CreateMap<PhotosForDetailDto, Photo>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<Users, UserForListDto>()
                .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(d => d.Age, opt => opt.MapFrom(d => d.DateOfBirth.CalculateAge()));

            CreateMap<UserForUpdateDto, Users>();
            //CreateMap<Users, UserForListDto>();
            //CreateMap<UserForListDto, Users>();

            CreateMap<Users, UserForRegisterDto>();
            //CreateMap<UserForRegisterDto, Users>();

            CreateMap<Users, UserForLoginDto>();
            //CreateMap<UserForLoginDto, Users>();



        }
    }
}
