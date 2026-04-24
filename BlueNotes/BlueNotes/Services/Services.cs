using BlueNotes.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;

namespace BlueNotes.Services;

// ═══════════════════════════════════════════════════════════════════════════════
// NOTEBOOK SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface INotebookService
{
    Task<List<Notebook>> GetAllAsync();
    Task<Notebook?>      GetByIdAsync(int id);
    Task<int>            SaveAsync(Notebook notebook);
    Task<int>            DeleteAsync(Notebook notebook);
    Task<int>            GetNoteCountAsync(int notebookId);
}

public class NotebookService : INotebookService
{
    private readonly DatabaseService _db;
    public NotebookService(DatabaseService db) => _db = db;

    public async Task<List<Notebook>> GetAllAsync()
    {
        var notebooks = await _db.Connection.Table<Notebook>()
            .OrderBy(n => n.Name).ToListAsync();
        foreach (var nb in notebooks)
            nb.NoteCount = await GetNoteCountAsync(nb.Id);
        return notebooks;
    }

    public async Task<Notebook?> GetByIdAsync(int id) =>
        await _db.Connection.Table<Notebook>().Where(n => n.Id == id).FirstOrDefaultAsync();

    public async Task<int> SaveAsync(Notebook notebook) =>
        notebook.Id == 0
            ? await _db.Connection.InsertAsync(notebook)
            : await _db.Connection.UpdateAsync(notebook);

    public async Task<int> DeleteAsync(Notebook notebook) =>
        await _db.Connection.DeleteAsync(notebook);

    public async Task<int> GetNoteCountAsync(int notebookId) =>
        await _db.Connection.Table<Note>()
            .Where(n => n.NotebookId == notebookId && !n.IsDeleted).CountAsync();
}

// ═══════════════════════════════════════════════════════════════════════════════
// TAG SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface ITagService
{
    Task<List<Tag>>  GetAllAsync();
    Task<int>        SaveAsync(Tag tag);
    Task<int>        DeleteAsync(Tag tag);
    Task             AddTagToNoteAsync(int noteId, int tagId);
    Task             RemoveTagFromNoteAsync(int noteId, int tagId);
    Task<List<Tag>>  GetTagsForNoteAsync(int noteId);
}

public class TagService : ITagService
{
    private readonly DatabaseService _db;
    public TagService(DatabaseService db) => _db = db;

    public async Task<List<Tag>> GetAllAsync()
    {
        var tags = await _db.Connection.Table<Tag>().OrderBy(t => t.Name).ToListAsync();
        foreach (var tag in tags)
            tag.UsageCount = await _db.Connection.Table<NoteTag>()
                .Where(nt => nt.TagId == tag.Id).CountAsync();
        return tags;
    }

    public async Task<int> SaveAsync(Tag tag) =>
        tag.Id == 0
            ? await _db.Connection.InsertAsync(tag)
            : await _db.Connection.UpdateAsync(tag);

    public async Task<int> DeleteAsync(Tag tag)
    {
        // remove associations first
        var links = await _db.Connection.Table<NoteTag>()
            .Where(nt => nt.TagId == tag.Id).ToListAsync();
        foreach (var l in links) await _db.Connection.DeleteAsync(l);
        return await _db.Connection.DeleteAsync(tag);
    }

    public async Task AddTagToNoteAsync(int noteId, int tagId)
    {
        var exists = await _db.Connection.Table<NoteTag>()
            .Where(nt => nt.NoteId == noteId && nt.TagId == tagId).FirstOrDefaultAsync();
        if (exists is null)
            await _db.Connection.InsertAsync(new NoteTag { NoteId = noteId, TagId = tagId });
    }

    public async Task RemoveTagFromNoteAsync(int noteId, int tagId)
    {
        var link = await _db.Connection.Table<NoteTag>()
            .Where(nt => nt.NoteId == noteId && nt.TagId == tagId).FirstOrDefaultAsync();
        if (link is not null) await _db.Connection.DeleteAsync(link);
    }

    public async Task<List<Tag>> GetTagsForNoteAsync(int noteId)
    {
        var links  = await _db.Connection.Table<NoteTag>().Where(nt => nt.NoteId == noteId).ToListAsync();
        var tagIds = links.Select(l => l.TagId).ToHashSet();
        var all    = await _db.Connection.Table<Tag>().ToListAsync();
        return all.Where(t => tagIds.Contains(t.Id)).ToList();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// SEARCH SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface ISearchService
{
    Task<List<Note>> SearchAsync(string query, int? notebookId = null, int? tagId = null);
}

public class SearchService : ISearchService
{
    private readonly DatabaseService _db;
    public SearchService(DatabaseService db) => _db = db;

    public async Task<List<Note>> SearchAsync(string query, int? notebookId = null, int? tagId = null)
    {
        if (string.IsNullOrWhiteSpace(query) && notebookId is null && tagId is null)
            return new List<Note>();

        var all = await _db.Connection.Table<Note>()
            .Where(n => !n.IsDeleted && !n.IsArchived).ToListAsync();

        if (!string.IsNullOrWhiteSpace(query))
        {
            var q = query.ToLowerInvariant();
            all = all.Where(n =>
                n.Title.ToLowerInvariant().Contains(q) ||
                n.Body.ToLowerInvariant().Contains(q)).ToList();
        }

        if (notebookId.HasValue)
            all = all.Where(n => n.NotebookId == notebookId.Value).ToList();

        if (tagId.HasValue)
        {
            var linked = await _db.Connection.Table<NoteTag>()
                .Where(nt => nt.TagId == tagId.Value).ToListAsync();
            var ids = linked.Select(l => l.NoteId).ToHashSet();
            all = all.Where(n => ids.Contains(n.Id)).ToList();
        }

        return all.OrderByDescending(n => n.UpdatedAt).ToList();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// EXPORT SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface IExportService
{
    Task<string> ExportAsTxtAsync(Note note);
    Task<string> ExportAsPdfAsync(Note note);
    Task         ShareAsync(string filePath);
}

public class ExportService : IExportService
{
    public async Task<string> ExportAsTxtAsync(Note note)
    {
        var fileName = $"{Sanitize(note.Title)}.txt";
        var path     = Path.Combine(FileSystem.CacheDirectory, fileName);
        var content  = $"{note.Title}\n{"=".PadRight(note.Title.Length, '=')}\n\n{note.Body}\n\n— Blue Notes {note.UpdatedAtFormatted}";
        await File.WriteAllTextAsync(path, content);
        return path;
    }

    public Task<string> ExportAsPdfAsync(Note note)
    {
        var doc  = new PdfDocument();
        var page = doc.AddPage();
        var gfx  = XGraphics.FromPdfPage(page);
        var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
        var bodyFont  = new XFont("Arial", 11, XFontStyle.Regular);

        gfx.DrawString(note.Title, titleFont, XBrushes.DarkBlue,
            new XRect(40, 40, page.Width - 80, 40), XStringFormats.TopLeft);
        gfx.DrawString(note.Body, bodyFont, XBrushes.Black,
            new XRect(40, 90, page.Width - 80, page.Height - 130), XStringFormats.TopLeft);

        var fileName = $"{Sanitize(note.Title)}.pdf";
        var path     = Path.Combine(FileSystem.CacheDirectory, fileName);
        doc.Save(path);
        return Task.FromResult(path);
    }

    public async Task ShareAsync(string filePath)
    {
        await Share.Default.RequestAsync(new ShareFileRequest
        {
            Title = "Compartilhar nota",
            File  = new ShareFile(filePath)
        });
    }

    private static string Sanitize(string name) =>
        string.Concat(name.Split(Path.GetInvalidFileNameChars())).Trim();
}

// ═══════════════════════════════════════════════════════════════════════════════
// SETTINGS SERVICE
// ═══════════════════════════════════════════════════════════════════════════════
public interface ISettingsService
{
    Task<string>  GetAsync(string key, string defaultValue = "");
    Task          SetAsync(string key, string value);
    Task<bool>    GetBoolAsync(string key, bool defaultValue = false);
    Task          SetBoolAsync(string key, bool value);
}

public class SettingsService : ISettingsService
{
    private readonly DatabaseService _db;
    public SettingsService(DatabaseService db) => _db = db;

    public async Task<string> GetAsync(string key, string defaultValue = "")
    {
        var s = await _db.Connection.Table<Setting>().Where(x => x.Key == key).FirstOrDefaultAsync();
        return s?.Value ?? defaultValue;
    }

    public async Task SetAsync(string key, string value)
    {
        var s = await _db.Connection.Table<Setting>().Where(x => x.Key == key).FirstOrDefaultAsync();
        if (s is null)
            await _db.Connection.InsertAsync(new Setting { Key = key, Value = value });
        else { s.Value = value; await _db.Connection.UpdateAsync(s); }
    }

    public async Task<bool> GetBoolAsync(string key, bool defaultValue = false)
    {
        var val = await GetAsync(key, defaultValue.ToString());
        return bool.TryParse(val, out var b) ? b : defaultValue;
    }

    public async Task SetBoolAsync(string key, bool value) =>
        await SetAsync(key, value.ToString());
}
