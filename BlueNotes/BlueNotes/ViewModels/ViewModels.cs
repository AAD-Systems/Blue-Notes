using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BlueNotes.Models;
using BlueNotes.Services;
using BlueNotes.Views.Pages;

namespace BlueNotes.ViewModels;

// ═══════════════════════════════════════════════════════════════════════════════
// NOTEBOOKS VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class NotebooksViewModel : BaseViewModel
{
    private readonly INotebookService _notebooks;
    public ObservableCollection<Notebook> Items { get; } = new();

    [ObservableProperty] private bool _isEmpty;

    public NotebooksViewModel(INotebookService notebooks)
    {
        _notebooks = notebooks;
        Title = "Cadernos";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _notebooks.GetAllAsync();
            Items.Clear();
            foreach (var nb in list) Items.Add(nb);
            IsEmpty = !Items.Any();
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task AddNotebookAsync()
    {
        string? name = await Shell.Current.DisplayPromptAsync(
            "Novo Caderno", "Nome do caderno:", "Criar", "Cancelar", "Ex: Receitas");
        if (string.IsNullOrWhiteSpace(name)) return;

        await _notebooks.SaveAsync(new Notebook { Name = name.Trim() });
        await LoadAsync();
    }

    [RelayCommand]
    private async Task RenameAsync(Notebook nb)
    {
        string? name = await Shell.Current.DisplayPromptAsync(
            "Renomear", "Novo nome:", "OK", "Cancelar", nb.Name);
        if (string.IsNullOrWhiteSpace(name)) return;
        nb.Name = name.Trim();
        await _notebooks.SaveAsync(nb);
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync(Notebook nb)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir", $"Excluir '{nb.Name}'?", "Excluir", "Cancelar");
        if (!confirm) return;
        await _notebooks.DeleteAsync(nb);
        await LoadAsync();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// SEARCH VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class SearchViewModel : BaseViewModel
{
    private readonly ISearchService _search;
    private CancellationTokenSource _cts = new();

    public ObservableCollection<Note>   Results       { get; } = new();
    public ObservableCollection<string> RecentQueries { get; } = new();

    [ObservableProperty] private string _query = string.Empty;
    [ObservableProperty] private bool   _hasResults;
    [ObservableProperty] private bool   _noResults;

    public SearchViewModel(ISearchService search)
    {
        _search = search;
        Title   = "Busca";
    }

    partial void OnQueryChanged(string value) => _ = DebounceSearchAsync(value);

    private async Task DebounceSearchAsync(string q)
    {
        _cts.Cancel();
        _cts = new CancellationTokenSource();
        try
        {
            await Task.Delay(300, _cts.Token);
            await SearchAsync(q);
        }
        catch (TaskCanceledException) { }
    }

    private async Task SearchAsync(string q)
    {
        if (string.IsNullOrWhiteSpace(q)) { Results.Clear(); HasResults = false; NoResults = false; return; }
        IsBusy = true;
        try
        {
            var list = await _search.SearchAsync(q);
            Results.Clear();
            foreach (var n in list) Results.Add(n);
            HasResults = Results.Any();
            NoResults  = !HasResults;

            if (!RecentQueries.Contains(q))
            {
                RecentQueries.Insert(0, q);
                if (RecentQueries.Count > 10) RecentQueries.RemoveAt(10);
            }
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task OpenNoteAsync(Note note) =>
        await Shell.Current.GoToAsync(nameof(NoteEditorPage),
            new Dictionary<string, object> { ["NoteId"] = note.Id });

    [RelayCommand]
    private void UseRecentQuery(string q) => Query = q;
}

// ═══════════════════════════════════════════════════════════════════════════════
// ARCHIVE VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class ArchiveViewModel : BaseViewModel
{
    private readonly INoteService _notes;
    public ObservableCollection<Note> Items { get; } = new();
    [ObservableProperty] private bool _isEmpty;

    public ArchiveViewModel(INoteService notes) { _notes = notes; Title = "Arquivo"; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _notes.GetArchivedAsync();
            Items.Clear(); foreach (var n in list) Items.Add(n);
            IsEmpty = !Items.Any();
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task UnarchiveAsync(Note note) { await _notes.ArchiveAsync(note, false); await LoadAsync(); }

    [RelayCommand]
    private async Task DeleteAsync(Note note) { await _notes.SoftDeleteAsync(note); await LoadAsync(); }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TRASH VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class TrashViewModel : BaseViewModel
{
    private readonly INoteService _notes;
    public ObservableCollection<Note> Items { get; } = new();
    [ObservableProperty] private bool _isEmpty;

    public TrashViewModel(INoteService notes) { _notes = notes; Title = "Lixeira"; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _notes.GetTrashedAsync();
            Items.Clear(); foreach (var n in list) Items.Add(n);
            IsEmpty = !Items.Any();
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task RestoreAsync(Note note) { await _notes.RestoreAsync(note); await LoadAsync(); }

    [RelayCommand]
    private async Task PermanentDeleteAsync(Note note)
    {
        bool ok = await Shell.Current.DisplayAlert(
            "Excluir permanentemente", "Esta ação não pode ser desfeita.", "Excluir", "Cancelar");
        if (ok) { await _notes.DeleteAsync(note); await LoadAsync(); }
    }

    [RelayCommand]
    private async Task EmptyTrashAsync()
    {
        bool ok = await Shell.Current.DisplayAlert(
            "Esvaziar Lixeira", "Todas as notas serão excluídas permanentemente.", "Esvaziar", "Cancelar");
        if (!ok) return;
        foreach (var n in Items.ToList()) await _notes.DeleteAsync(n);
        await LoadAsync();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TAGS VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class TagsViewModel : BaseViewModel
{
    private readonly ITagService _tags;
    public ObservableCollection<Tag> Items { get; } = new();
    [ObservableProperty] private bool _isEmpty;

    public TagsViewModel(ITagService tags) { _tags = tags; Title = "Etiquetas"; }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsBusy = true;
        try
        {
            var list = await _tags.GetAllAsync();
            Items.Clear(); foreach (var t in list) Items.Add(t);
            IsEmpty = !Items.Any();
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task AddTagAsync()
    {
        string? name = await Shell.Current.DisplayPromptAsync(
            "Nova Etiqueta", "Nome:", "Criar", "Cancelar", "Ex: importante");
        if (string.IsNullOrWhiteSpace(name)) return;
        await _tags.SaveAsync(new Tag { Name = name.Trim() });
        await LoadAsync();
    }

    [RelayCommand]
    private async Task DeleteTagAsync(Tag tag)
    {
        bool ok = await Shell.Current.DisplayAlert(
            "Excluir", $"Excluir etiqueta '{tag.Name}'?", "Excluir", "Cancelar");
        if (ok) { await _tags.DeleteAsync(tag); await LoadAsync(); }
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// SETTINGS VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class SettingsViewModel : BaseViewModel
{
    private readonly ISettingsService _settings;

    [ObservableProperty] private string _theme        = "dark";
    [ObservableProperty] private string _language     = "pt-BR";
    [ObservableProperty] private string _defaultSort  = "date";
    [ObservableProperty] private bool   _autoSave     = true;
    [ObservableProperty] private bool   _biometric    = false;
    [ObservableProperty] private int    _trashDays    = 30;

    public SettingsViewModel(ISettingsService settings)
    {
        _settings = settings;
        Title     = "Configurações";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        Theme       = await _settings.GetAsync(SettingKeys.Theme, "dark");
        Language    = await _settings.GetAsync(SettingKeys.Language, "pt-BR");
        DefaultSort = await _settings.GetAsync(SettingKeys.DefaultSort, "date");
        AutoSave    = await _settings.GetBoolAsync(SettingKeys.AutoSave, true);
        Biometric   = await _settings.GetBoolAsync(SettingKeys.BiometricLock, false);
        int.TryParse(await _settings.GetAsync(SettingKeys.TrashDays, "30"), out var d);
        TrashDays = d;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        await _settings.SetAsync(SettingKeys.Theme,    Theme);
        await _settings.SetAsync(SettingKeys.Language, Language);
        await _settings.SetAsync(SettingKeys.DefaultSort, DefaultSort);
        await _settings.SetBoolAsync(SettingKeys.AutoSave, AutoSave);
        await _settings.SetBoolAsync(SettingKeys.BiometricLock, Biometric);
        await _settings.SetAsync(SettingKeys.TrashDays, TrashDays.ToString());
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PROFILE VIEW MODEL
// ═══════════════════════════════════════════════════════════════════════════════
public partial class ProfileViewModel : BaseViewModel
{
    private readonly INoteService      _notes;
    private readonly INotebookService  _notebooks;
    private readonly ITagService       _tags;

    [ObservableProperty] private int _totalNotes;
    [ObservableProperty] private int _totalNotebooks;
    [ObservableProperty] private int _totalTags;
    [ObservableProperty] private string _appVersion = "0.0.0.0.1";

    public ProfileViewModel(INoteService notes, INotebookService notebooks, ITagService tags)
    {
        _notes     = notes;
        _notebooks = notebooks;
        _tags      = tags;
        Title      = "Perfil";
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        var allNotes  = await _notes.GetAllAsync();
        var notebooks = await _notebooks.GetAllAsync();
        var tags      = await _tags.GetAllAsync();

        TotalNotes     = allNotes.Count;
        TotalNotebooks = notebooks.Count;
        TotalTags      = tags.Count;
    }
}
