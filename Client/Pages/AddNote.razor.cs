using Client.Exceptions;
using Client.Models.Forms;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Pages;

public partial class AddNote : ComponentBase
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required INoteService NoteService { get; set; }

    private readonly AddNoteFormModel _model = new();
    private readonly List<string> _errors = [];
    private bool _isLoading = true;

    private async Task SubmitAsync()
    {
        _errors.Clear();
        _isLoading = true;

        try
        {
            var result = await NoteService.AddNoteAsync(_model.Title, _model.Content);
            if (result.IsSuccess)
            {
                _model.Title = string.Empty;
                _model.Content = string.Empty;
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
}