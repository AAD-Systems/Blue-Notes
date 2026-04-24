using BlueNotes.ViewModels;
namespace BlueNotes.Views.Pages;

public partial class NotebooksPage : ContentPage
{
    public NotebooksPage(NotebooksViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.Opacity = 0;
        await this.FadeTo(1, 250, Easing.CubicOut);
    }
}

public partial class SearchPage : ContentPage
{
    public SearchPage(SearchViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

public partial class ArchivePage : ContentPage
{
    public ArchivePage(ArchiveViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.Opacity = 0;
        await this.FadeTo(1, 250, Easing.CubicOut);
    }
}

public partial class TrashPage : ContentPage
{
    public TrashPage(TrashViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

public partial class TagsPage : ContentPage
{
    public TagsPage(TagsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.Opacity = 0;
        await this.FadeTo(1, 250, Easing.CubicOut);
    }
}

public partial class SettingsPage : ContentPage
{
    public SettingsPage(SettingsViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.TranslationY = 30;
        this.Opacity = 0;
        await Task.WhenAll(
            this.TranslateTo(0, 0, 300, Easing.CubicOut),
            this.FadeTo(1, 250));
    }
}
