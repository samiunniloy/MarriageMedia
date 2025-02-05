using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // Mapping for Photo to PhotoDto
            CreateMap<Photo, PhotoDto>();

            // Mapping for AppUser to MemberDto
            CreateMap<AppUser, MemberDto>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(
                    src => src.Photos.FirstOrDefault(x => x.IsMain)!.Url));

            // Two-way mapping for MemberUpdateDto and AppUser
            CreateMap<MemberUpdateDto, AppUser>().ReverseMap();

            // Mapping for RegisterDto to AppUser
            CreateMap<RegisterDto, AppUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => DateOnly.Parse(src.DateOfBirth)))
                .ForMember(dest => dest.KnownAs, opt => opt.MapFrom(src => src.KnownAs))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender))
                .ForMember(dest => dest.Created, opt => opt.Ignore()) // Default initialization
                .ForMember(dest => dest.LastActive, opt => opt.Ignore()); // Default initialization



            //CreateMap<Message, MessageDto>()
            //    .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(
            //        src => src.Sender.Photos.FirstOrDefault(x => x.IsMain)!.Url))
            //    .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(
            //        src => src.Recipient.Photos.FirstOrDefault(x => x.IsMain)!.Url));

           CreateMap<Message, MessageDto>()
            .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(
                src => src.Sender.Photos.FirstOrDefault(p => p.IsMain)!.Url))  // Assuming you have a Photos relationship
            .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(
                src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain)!.Url)); // Same for Recipient

        }
    }
}
