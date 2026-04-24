using SQLite;

namespace BlueNotes.Models;

[Table("Settings")]
public class Setting
{
    [PrimaryKey]
    public string Key   { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

/// <summary>Well-known setting keys used across the app.</summary>
public static class SettingKeys
{
    public const string Theme         = "theme";          // dark | light | system
    public const string Language      = "language";       // pt-BR | en
    public const string DefaultSort   = "default_sort";   // date | title | color
    public const string AutoSave      = "auto_save";      // true | false
    public const string AutoSaveDelay = "auto_save_delay"; // seconds (3)
    public const string LockPin       = "lock_pin";       // 4-digit PIN hash
    public const string BiometricLock = "biometric_lock"; // true | false
    public const string TrashDays     = "trash_days";     // default 30
}
