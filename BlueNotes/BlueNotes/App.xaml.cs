using BlueNotes.Services;

namespace BlueNotes;

public partial class App : Application
{
    private readonly DatabaseService _db;

    public App(DatabaseService db)
    {
        InitializeComponent();
        _db = db;
        MainPage = new AppShell();
    }

    protected override async void OnStart()
    {
        base.OnStart();
        await _db.InitializeAsync();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Title = "Blue Notes";
        return window;
    }
}
