using Client.Exceptions;
using Client.Models;
using Client.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Client.Pages;

public partial class Notes : ComponentBase
{
    [Inject] public required NavigationManager NavigationManager { get; set; }
    [Inject] public required INoteService NoteService { get; set; }

    private readonly List<NoteModel> _notes = [];

    private readonly List<string> _errors = [];
    private bool _loadingNotes = true;
    private Guid _askConfirmDeletionNoteId = Guid.Empty;
    private Guid _awaitingDeletionNoteId = Guid.Empty;
    private NoteModel? _shownEditableNote;
    private bool _showAddNote;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            await LoadNotesAsync();
        }
        finally
        {
            _loadingNotes = false;
        }
    }

    private async Task DeleteNoteAsync(Guid noteId)
    {
        if (_askConfirmDeletionNoteId != noteId)
        {
            _askConfirmDeletionNoteId = noteId;
            StateHasChanged();
            return;
        }

        _awaitingDeletionNoteId = noteId;
        await Task.Yield();
        
        try
        {
            var result = await NoteService.DeleteNoteAsync(noteId);
            if (result.IsSuccess)
            {
                RemoveNoteFromLocalList(noteId);
                return;
            }

            //!apiDeleteNoteResponse.IsSuccess;
            _errors.Add((string.IsNullOrEmpty(result.ErrorMessage)
                ? "Unexpected error deleting note"
                : result.ErrorMessage));
        }
        finally
        {
            _awaitingDeletionNoteId = Guid.Empty;
            _askConfirmDeletionNoteId = Guid.Empty;
        }
    }

    private async Task LoadNotesAsync()
    {
        var page = 1;
        const int pageSize = 20;
        var hasMore = true;

        while (hasMore)
        {
            try
            {
                var result = await NoteService.GetNotesPageAsync(page, pageSize);
                if (result.IsSuccess)
                {
                    _notes.AddRange(result.Data.notes);
                    StateHasChanged();

                    hasMore = result.Data.hasMore;
                    page++;
                    continue;
                }

                _errors.Add((string.IsNullOrEmpty(result.ErrorMessage)
                    ? "Unexpected error loading notes"
                    : result.ErrorMessage));
                break;
            }
            catch (EncryptionKeyMissingException)
            {
                NavigationManager.NavigateTo("/login");
                break;
            }
        }
    }

    private void RemoveNoteFromLocalList(Guid noteId)
    {
        var removed = _notes.FirstOrDefault(n => n.Id == noteId);
        if (removed != null) _notes.Remove(removed);
    }

    private void HandleOpenAddNote() => _showAddNote = true;

    private void HandleCloseAddNote(NoteModel? addedNote)
    {
        if (addedNote is not null) _notes.Add(addedNote);
        _showAddNote = false;
    }
    private void HandleOpenEditableNote(NoteModel note) => _shownEditableNote = note;

    private void HandleCloseEditableNote(NoteModel? editedNote)
    {
        if (editedNote is not null)
        {
            var index = _notes.FindIndex(n => n.Id == editedNote.Id);
            if (index != -1) _notes[index] = editedNote;
        }
        _shownEditableNote = null;
    }
}