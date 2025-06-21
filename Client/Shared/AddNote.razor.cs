using Client.Exceptions;
using Client.Models;
using Client.Models.Forms;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Shared;

public partial class AddNote : ComponentBase
{
    [Parameter] public required EventCallback<NoteModel?> OnClose { get; set; }
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required INoteService NoteService { get; set; }

    private readonly NoteFormModel _model = new();
    private readonly List<string> _errors = [];
    private bool _isLoading;

    private async Task SubmitAsync()
    {
        _errors.Clear();
        _isLoading = true;
        await Task.Yield();

        try
        {
            var result = await NoteService.AddNoteAsync(_model.Title, _model.Content);
            if (result is {IsSuccess: true, Data: not null})
            {
                await OnClose.InvokeAsync(result.Data);
                return;
            }

            _errors.Add((string.IsNullOrEmpty(result.ErrorMessage)
                ? "Unexpected error during note creation"
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