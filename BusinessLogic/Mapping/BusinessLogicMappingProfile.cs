using AutoMapper;
using Core.Entities;
using Shared.Dto;
using Shared.Dto.Requests;

namespace BusinessLogic.Mapping;

public class BusinessLogicMappingProfile : Profile
{
    public BusinessLogicMappingProfile()
    {
        CreateMap<UserDto, UserEntity>();
        CreateMap<SignupRequest, UserDto>();
        
        CreateMap<NoteDto, NoteEntity>();
        CreateMap<NoteEntity, NoteDto>();
    }
}