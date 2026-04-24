using SQLite;
using BlueNotes.Models;

namespace BlueNotes.Services;

public class DatabaseService
{
    private SQLiteAsyncConnection? _db;
    private static readonly string _dbPath =
        Path.Combine(FileSystem.AppDataDirectory, "bluenotes.db3");

    public async Task InitializeAsync()
    {
        if (_db is not null) return;

        _db = new SQLiteAsyncConnection(_dbPath,
            SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _db.CreateTableAsync<Note>();
        await _db.CreateTableAsync<Notebook>();
        await _db.CreateTableAsync<Tag>();
        await _db.CreateTableAsync<NoteTag>();
        await _db.CreateTableAsync<Setting>();

        await SeedDefaultsAsync();
    }

    public SQLiteAsyncConnection Connection =>
        _db ?? throw new InvalidOperationException("DB not initialized. Call InitializeAsync first.");

    private async Task SeedDefaultsAsync()
    {
        var count = await _db!.Table<Notebook>().CountAsync();
        if (count > 0) return;

        await _db.InsertAsync(new Notebook { Name = "Geral", Color = "#2E86C1", Icon = "📓" });
        await _db.InsertAsync(new Notebook { Name = "Pessoal", Color = "#1A5276", Icon = "👤" });
        await _db.InsertAsync(new Notebook { Name = "Trabalho", Color = "#0B4F6C", Icon = "💼" });

        await _db.InsertAsync(new Setting { Key = SettingKeys.Theme,    Value = "dark" });
        await _db.InsertAsync(new Setting { Key = SettingKeys.Language, Value = "pt-BR" });
        await _db.InsertAsync(new Setting { Key = SettingKeys.DefaultSort, Value = "date" });
        await _db.InsertAsync(new Setting { Key = SettingKeys.AutoSave,    Value = "true" });
        await _db.InsertAsync(new Setting { Key = SettingKeys.AutoSaveDelay, Value = "3" });
        await _db.InsertAsync(new Setting { Key = SettingKeys.TrashDays,   Value = "30" });
    }
}
