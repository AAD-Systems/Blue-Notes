using BlueNotes.ViewModels;

namespace BlueNotes.Views.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;

    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Animate page entrance
        this.Opacity = 0;
        await this.FadeTo(1, 280, Easing.CubicOut);

        await _vm.LoadCommand.ExecuteAsync(null);
    }
}
