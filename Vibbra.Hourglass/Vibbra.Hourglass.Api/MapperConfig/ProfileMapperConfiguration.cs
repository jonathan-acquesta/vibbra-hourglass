using AutoMapper;
using Vibbra.Hourglass.Api.DTOs;
using Vibbra.Hourglass.Domain.Domains;

namespace Vibbra.Hourglass.Api.MapperConfig
{
    public class ProfileMapperConfiguration : Profile
    {
        public ProfileMapperConfiguration()
        {
            CreateMap<LoginRequestDTO, UserDomain>();

            CreateMap<UserDomain, UserResponseDTO>();
            CreateMap<UserRequestDTO, UserDomain>();

            CreateMap<ProjectDomain, ProjectResponseDTO>().ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users.Select(u => u.ID).ToList()));
            CreateMap<ProjectRequestDTO, ProjectDomain>().ForMember(dest => dest.Users, opt => opt.MapFrom(src => src.Users.Select(u => new UserDomain { ID = u }).ToList()));

            CreateMap<TimeDomain, TimeResponseDTO>();
            CreateMap<TimeRequestDTO, TimeDomain>();
        }
    }
}
