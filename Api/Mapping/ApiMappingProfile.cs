using AutoMapper;
using Shared.Dto;
using Shared.Dto.Requests;
using Shared.Dto.Requests.Auth;
using Shared.Dto.Requests.Notes;

namespace EncryptedNotes.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<SignupRequest, UserDto>();
        
        CreateMap<UpsertNoteRequest, NoteDto>();
    }
}