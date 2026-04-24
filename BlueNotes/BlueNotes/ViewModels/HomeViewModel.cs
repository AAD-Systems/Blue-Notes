using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BlueNotes.Models;
using BlueNotes.Services;
using BlueNotes.Views.Pages;

namespace BlueNotes.ViewModels;

public partial class HomeViewModel : BaseViewModel
{
    private readonly INoteService _notes;

    public ObservableCollection<Note> PinnedNotes  { get; } = new();
    public ObservableCollection<Note> RecentNotes  { get; } = new();

    [ObservableProperty] private bool _hasPinned;
    [ObservableProperty] private bool _isEmpty;

    public HomeViewModel(INoteService notes)
    {
        _notes = notes;
        Title  = "Início";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            await _notes.PurgeOldTrashAsync();

            var all    = await _notes.GetAllAsync();
            var pinned = all.Where(n => n.IsPinned).ToList();
            var recent = all.Where(n => !n.IsPinned)
                            .OrderByDescending(n => n.UpdatedAt).ToList();

            PinnedNotes.Clear();
            foreach (var n in pinned) PinnedNotes.Add(n);

            RecentNotes.Clear();
            foreach (var n in recent) RecentNotes.Add(n);

            HasPinned = PinnedNotes.Any();
            IsEmpty   = !all.Any();
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task NewNoteAsync() =>
        await Shell.Current.GoToAsync(nameof(NoteEditorPage));

    [RelayCommand]
    private async Task OpenNoteAsync(Note note) =>
        await Shell.Current.GoToAsync(nameof(NoteEditorPage),
            new Dictionary<string, object> { ["NoteId"] = note.Id });

    [RelayCommand]
    private async Task TogglePinAsync(Note note)
    {
        await _notes.PinAsync(note, !note.IsPinned);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task ArchiveNoteAsync(Note note)
    {
        await _notes.ArchiveAsync(note, true);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteNoteAsync(Note note)
    {
        await _notes.SoftDeleteAsync(note);
        await LoadAsync();
    }
}
