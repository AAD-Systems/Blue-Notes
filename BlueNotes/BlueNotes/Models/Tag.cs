using SQLite;

namespace BlueNotes.Models;

[Table("Tags")]
public class Tag
{
    [PrimaryKey, AutoIncrement]
    public int    Id    { get; set; }
    public string Name  { get; set; } = string.Empty;
    public string Color { get; set; } = "#2E86C1";

    [Ignore] public int UsageCount { get; set; }
}

[Table("NoteTags")]
public class NoteTag
{
    [PrimaryKey, AutoIncrement]
    public int NoteTagId { get; set; }
    public int NoteId    { get; set; }
    public int TagId     { get; set; }
}
