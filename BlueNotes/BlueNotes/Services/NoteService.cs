using BlueNotes.Models;

namespace BlueNotes.Services;

// ── Interface ─────────────────────────────────────────────────────────────────
public interface INoteService
{
    Task<List<Note>> GetAllAsync();
    Task<List<Note>> GetPinnedAsync();
    Task<List<Note>> GetArchivedAsync();
    Task<List<Note>> GetTrashedAsync();
    Task<Note?>      GetByIdAsync(int id);
    Task<int>        SaveAsync(Note note);
    Task<int>        DeleteAsync(Note note);
    Task             PinAsync(Note note, bool pin);
    Task             ArchiveAsync(Note note, bool archive);
    Task             SoftDeleteAsync(Note note);
    Task             RestoreAsync(Note note);
    Task             PurgeOldTrashAsync(int days = 30);
    Task<List<Note>> GetByNotebookAsync(int notebookId);
    Task<List<Note>> GetByTagAsync(int tagId);
}

// ── Implementation ────────────────────────────────────────────────────────────
public class NoteService : INoteService
{
    private readonly DatabaseService _db;

    public NoteService(DatabaseService db) => _db = db;

    public async Task<List<Note>> GetAllAsync() =>
        await _db.Connection.Table<Note>()
            .Where(n => !n.IsArchived && !n.IsDeleted)
            .OrderByDescending(n => n.IsPinned)
            .ToListAsync();

    public async Task<List<Note>> GetPinnedAsync() =>
        await _db.Connection.Table<Note>()
            .Where(n => n.IsPinned && !n.IsArchived && !n.IsDeleted)
            .ToListAsync();

    public async Task<List<Note>> GetArchivedAsync() =>
        await _db.Connection.Table<Note>()
            .Where(n => n.IsArchived && !n.IsDeleted)
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();

    public async Task<List<Note>> GetTrashedAsync() =>
        await _db.Connection.Table<Note>()
            .Where(n => n.IsDeleted)
            .OrderByDescending(n => n.DeletedAt)
            .ToListAsync();

    public async Task<Note?> GetByIdAsync(int id) =>
        await _db.Connection.Table<Note>().Where(n => n.Id == id).FirstOrDefaultAsync();

    public async Task<int> SaveAsync(Note note)
    {
        note.UpdatedAt = DateTime.UtcNow;
        if (note.Id == 0)
            return await _db.Connection.InsertAsync(note);
        return await _db.Connection.UpdateAsync(note);
    }

    public async Task<int> DeleteAsync(Note note) =>
        await _db.Connection.DeleteAsync(note);

    public async Task PinAsync(Note note, bool pin)
    {
        note.IsPinned = pin;
        await SaveAsync(note);
    }

    public async Task ArchiveAsync(Note note, bool archive)
    {
        note.IsArchived = archive;
        await SaveAsync(note);
    }

    public async Task SoftDeleteAsync(Note note)
    {
        note.IsDeleted = true;
        note.DeletedAt = DateTime.UtcNow;
        await SaveAsync(note);
    }

    public async Task RestoreAsync(Note note)
    {
        note.IsDeleted  = false;
        note.IsArchived = false;
        note.DeletedAt  = null;
        await SaveAsync(note);
    }

    public async Task PurgeOldTrashAsync(int days = 30)
    {
        var cutoff = DateTime.UtcNow.AddDays(-days);
        var old = await _db.Connection.Table<Note>()
            .Where(n => n.IsDeleted && n.DeletedAt < cutoff)
            .ToListAsync();
        foreach (var n in old) await _db.Connection.DeleteAsync(n);
    }

    public async Task<List<Note>> GetByNotebookAsync(int notebookId) =>
        await _db.Connection.Table<Note>()
            .Where(n => n.NotebookId == notebookId && !n.IsDeleted && !n.IsArchived)
            .OrderByDescending(n => n.UpdatedAt)
            .ToListAsync();

    public async Task<List<Note>> GetByTagAsync(int tagId)
    {
        var noteTags = await _db.Connection.Table<NoteTag>()
            .Where(nt => nt.TagId == tagId)
            .ToListAsync();
        var noteIds = noteTags.Select(nt => nt.NoteId).ToHashSet();
        var all = await _db.Connection.Table<Note>()
            .Where(n => !n.IsDeleted && !n.IsArchived)
            .ToListAsync();
        return all.Where(n => noteIds.Contains(n.Id)).ToList();
    }
}
