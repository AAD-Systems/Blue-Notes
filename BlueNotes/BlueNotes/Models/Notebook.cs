using SQLite;

namespace BlueNotes.Models;

[Table("Notebooks")]
public class Notebook
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Name      { get; set; } = string.Empty;
    public string Color     { get; set; } = "#2E86C1";
    public string Icon      { get; set; } = "📓";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Ignore] public int NoteCount { get; set; }
}
