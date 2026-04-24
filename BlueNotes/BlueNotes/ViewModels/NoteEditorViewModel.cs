using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BlueNotes.Models;
using BlueNotes.Services;

namespace BlueNotes.ViewModels;

[QueryProperty(nameof(NoteId), "NoteId")]
public partial class NoteEditorViewModel : BaseViewModel
{
    private readonly INoteService    _notes;
    private readonly ITagService     _tags;
    private readonly IExportService  _export;
    private readonly ISettingsService _settings;

    private Timer?  _autoSaveTimer;
    private bool    _isDirty;
    private Note    _note = new();

    [ObservableProperty] private int    _noteId;
    [ObservableProperty] private string _noteTitle = string.Empty;
    [ObservableProperty] private string _noteBody  = string.Empty;
    [ObservableProperty] private string _noteColor = "#1B2A3B";
    [ObservableProperty] private string _fontSize  = "Medium";
    [ObservableProperty] private bool   _isLocked;
    [ObservableProperty] private bool   _isPinned;
    [ObservableProperty] private int    _wordCount;
    [ObservableProperty] private int    _charCount;
    [ObservableProperty] private string _lastSaved = "Não salvo";
    [ObservableProperty] private bool   _isNewNote;

    public ObservableCollection<Tag> NoteTags     { get; } = new();
    public ObservableCollection<Tag> AllTags      { get; } = new();
    public ObservableCollection<string> ColorOptions { get; } = new()
    {
        "#1B2A3B", "#0B4F6C", "#154360", "#1A5276",
        "#2E4057", "#2C3E50", "#17202A", "#212F3D"
    };

    public NoteEditorViewModel(INoteService notes, ITagService tags,
        IExportService export, ISettingsService settings)
    {
        _notes    = notes;
        _tags     = tags;
        _export   = export;
        _settings = settings;
    }

    partial void OnNoteIdChanged(int value) => _ = LoadNoteAsync(value);

    private async Task LoadNoteAsync(int id)
    {
        IsBusy = true;
        try
        {
            if (id == 0)
            {
                _note   = new Note();
                IsNewNote = true;
                Title   = "Nova Nota";
            }
            else
            {
                _note     = await _notes.GetByIdAsync(id) ?? new Note();
                IsNewNote = false;
                Title     = "Editar Nota";
            }

            NoteTitle = _note.Title;
            NoteBody  = _note.Body;
            NoteColor = _note.Color;
            FontSize  = _note.FontSize;
            IsLocked  = _note.IsLocked;
            IsPinned  = _note.IsPinned;
            UpdateCounts();

            var noteTags = await _tags.GetTagsForNoteAsync(_note.Id);
            NoteTags.Clear();
            foreach (var t in noteTags) NoteTags.Add(t);

            var allAutoSave = await _settings.GetBoolAsync(SettingKeys.AutoSave, true);
            if (allAutoSave) StartAutoSave();
        }
        finally { IsBusy = false; }
    }

    partial void OnNoteTitleChanged(string value) => MarkDirty();
    partial void OnNoteBodyChanged(string value)
    {
        UpdateCounts();
        MarkDirty();
    }

    private void UpdateCounts()
    {
        CharCount = NoteBody.Length;
        WordCount = string.IsNullOrWhiteSpace(NoteBody)
            ? 0 : NoteBody.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;
    }

    private void MarkDirty() => _isDirty = true;

    private void StartAutoSave()
    {
        _autoSaveTimer?.Dispose();
        _autoSaveTimer = new Timer(async _ =>
        {
            if (_isDirty) await SaveAsync();
        }, null, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        _note.Title    = NoteTitle;
        _note.Body     = NoteBody;
        _note.Color    = NoteColor;
        _note.FontSize = FontSize;
        _note.IsLocked = IsLocked;
        _note.IsPinned = IsPinned;

        await _notes.SaveAsync(_note);
        _isDirty  = false;
        LastSaved = $"Salvo às {DateTime.Now:HH:mm}";
    }

    [RelayCommand]
    private async Task ExportTxtAsync()
    {
        await SaveAsync();
        var path = await _export.ExportAsTxtAsync(_note);
        await _export.ShareAsync(path);
    }

    [RelayCommand]
    private async Task ExportPdfAsync()
    {
        await SaveAsync();
        var path = await _export.ExportAsPdfAsync(_note);
        await _export.ShareAsync(path);
    }

    [RelayCommand]
    private async Task ToggleLockAsync()
    {
        IsLocked = !IsLocked;
        await SaveAsync();
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        if (_isDirty) await SaveAsync();
        _autoSaveTimer?.Dispose();
        await Shell.Current.GoToAsync("..");
    }
}
