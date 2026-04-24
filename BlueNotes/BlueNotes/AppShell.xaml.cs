using BlueNotes.Views.Pages;

namespace BlueNotes;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Register routes for navigation
        Routing.RegisterRoute(nameof(NoteEditorPage), typeof(NoteEditorPage));
        Routing.RegisterRoute(nameof(HomePage),       typeof(HomePage));
        Routing.RegisterRoute(nameof(NotebooksPage),  typeof(NotebooksPage));
        Routing.RegisterRoute(nameof(TagsPage),       typeof(TagsPage));
        Routing.RegisterRoute(nameof(SearchPage),     typeof(SearchPage));
        Routing.RegisterRoute(nameof(ArchivePage),    typeof(ArchivePage));
        Routing.RegisterRoute(nameof(TrashPage),      typeof(TrashPage));
        Routing.RegisterRoute(nameof(SettingsPage),   typeof(SettingsPage));
        Routing.RegisterRoute(nameof(ProfilePage),    typeof(ProfilePage));
    }
}
