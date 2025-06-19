using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Shared.Constants;
using Shared.Validations;

namespace Shared.Dto.Requests.Notes;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public record UpsertNoteRequest
{
    [Required]
    [ValidBase64]
    [CustomStringLength(NoteConstants.EncryptedTitleBase64MaxLength, 1)]
    public required string EncryptedTitleBase64 { get; init; }
    [Required]
    [ValidBase64]
    [CustomStringLength(NoteConstants.EncryptedContentBase64MaxLength, 1)]
    public required string EncryptedContentBase64 { get; init; }
}