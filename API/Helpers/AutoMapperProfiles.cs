using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles:Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Photo, PhotoDto>();
            CreateMap<AppUser, MemberDto>()
                 .ForMember(d => d.PhotoUrl, o => o.MapFrom(
                  s => s.Photos.FirstOrDefault(x => x.IsMain)!.Url));

            CreateMap<MemberUpdateDto, AppUser>().ReverseMap();


           // CreateMap<RegisterDto, AppUser>()
           //.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
           //.ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.DateOfBirth)))
           //.ForMember(dest => dest.KnownAs, opt => opt.MapFrom(src => src.KnownAs))
           //.ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
           //.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
           //.ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender));


        }
    }
}
