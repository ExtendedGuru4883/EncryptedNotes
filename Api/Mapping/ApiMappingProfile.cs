using AutoMapper;
using Shared.Dto;
using Shared.Dto.Requests;

namespace EncryptedNotes.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<SignupRequest, UserDto>();
    }
}