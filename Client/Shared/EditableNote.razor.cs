using Client.Exceptions;
using Client.Models;
using Client.Models.Forms;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Shared;

public partial class EditableNote : ComponentBase
{
    [Parameter] public required EventCallback<NoteModel?> OnClose { get; set; }
    [Parameter] public required NoteModel Note { get; set; }
    [Inject] public required INoteService NoteService { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }

    private readonly NoteFormModel _noteFormModel = new();

    private readonly List<string> _errors = [];
    private bool _isLoading;

    protected override void OnParametersSet()
    {
        _noteFormModel.Title = Note.Title;
        _noteFormModel.Content = Note.Content;
    }

    private async Task SubmitEdit(NoteFormModel noteForm)
    {
        _errors.Clear();
        _isLoading = true;
        await Task.Yield();

        try
        {
            var result = await NoteService.UpdateNoteAsync(Note.Id, noteForm.Title, noteForm.Content);
            if (result is { IsSuccess: true, Data: not null })
            {
                await OnClose.InvokeAsync(result.Data);
                return;
            }

            _errors.Add((string.IsNullOrEmpty(result.ErrorMessage)
                ? "Unexpected error updating note"
                : result.ErrorMessage));
        }
        catch (EncryptionKeyMissingException)
        {
            NavigationManager.NavigateTo("/login");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void HandleClose() => OnClose.InvokeAsync(null);
}