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

            CreateMap<ProjectDomain, ProjectResponseDTO>();
            CreateMap<ProjectRequestPostDTO, ProjectDomain>();
            CreateMap<ProjectRequestPutDTO, ProjectDomain>();

            CreateMap<TimeDomain, TimeResponseDTO>();
            CreateMap<TimeRequestPostDTO, TimeDomain>();
            CreateMap<TimeRequestPutDTO, TimeDomain>();
        }
    }
}
