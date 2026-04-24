using SQLite;

namespace BlueNotes.Models;

[Table("Notes")]
public class Note
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string Title       { get; set; } = string.Empty;
    public string Body        { get; set; } = string.Empty;
    public string Color       { get; set; } = "#1B2A3B";   // default card color
    public string FontSize    { get; set; } = "Medium";    // Small | Medium | Large

    public int    NotebookId  { get; set; } = 0;

    public bool   IsPinned    { get; set; } = false;
    public bool   IsArchived  { get; set; } = false;
    public bool   IsDeleted   { get; set; } = false;
    public bool   IsLocked    { get; set; } = false;

    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Computed – not stored in DB
    [Ignore] public int WordCount => string.IsNullOrWhiteSpace(Body)
        ? 0
        : Body.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

    [Ignore] public string UpdatedAtFormatted =>
        UpdatedAt.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    [Ignore] public string PreviewBody =>
        Body.Length > 120 ? Body[..120].TrimEnd() + "…" : Body;
}
