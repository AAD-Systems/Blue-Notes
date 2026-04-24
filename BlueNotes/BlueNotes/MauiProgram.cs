using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using BlueNotes.Services;
using BlueNotes.ViewModels;
using BlueNotes.Views.Pages;

namespace BlueNotes;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
                fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                fonts.AddFont("Nunito-ExtraBold.ttf", "NunitoExtraBold");
                fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
            });

        // ── Services ──────────────────────────────────────────────────
        builder.Services.AddSingleton<DatabaseService>();
        builder.Services.AddSingleton<INoteService, NoteService>();
        builder.Services.AddSingleton<INotebookService, NotebookService>();
        builder.Services.AddSingleton<ITagService, TagService>();
        builder.Services.AddSingleton<ISearchService, SearchService>();
        builder.Services.AddSingleton<IExportService, ExportService>();
        builder.Services.AddSingleton<ISettingsService, SettingsService>();

        // ── ViewModels ────────────────────────────────────────────────
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<NoteEditorViewModel>();
        builder.Services.AddTransient<NotebooksViewModel>();
        builder.Services.AddTransient<SearchViewModel>();
        builder.Services.AddTransient<ArchiveViewModel>();
        builder.Services.AddTransient<TrashViewModel>();
        builder.Services.AddTransient<SettingsViewModel>();
        builder.Services.AddTransient<TagsViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // ── Pages ─────────────────────────────────────────────────────
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<NoteEditorPage>();
        builder.Services.AddTransient<NotebooksPage>();
        builder.Services.AddTransient<SearchPage>();
        builder.Services.AddTransient<ArchivePage>();
        builder.Services.AddTransient<TrashPage>();
        builder.Services.AddTransient<SettingsPage>();
        builder.Services.AddTransient<TagsPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
