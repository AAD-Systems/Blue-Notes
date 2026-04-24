using BlueNotes.ViewModels;

namespace BlueNotes.Views.Pages;

public partial class NoteEditorPage : ContentPage
{
    private readonly NoteEditorViewModel _vm;

    public NoteEditorPage(NoteEditorViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        this.TranslationX = 60;
        this.Opacity = 0;
        await Task.WhenAll(
            this.TranslateTo(0, 0, 300, Easing.CubicOut),
            this.FadeTo(1, 250, Easing.CubicOut));
    }

    // ── Font Size Picker ──────────────────────────────────────────────────────
    private async void OnFontSizeTap(object sender, EventArgs e)
    {
        string result = await DisplayActionSheet(
            "Tamanho da fonte", "Cancelar", null, "Pequeno", "Médio", "Grande");
        if (result is null || result == "Cancelar") return;
        _vm.FontSize = result switch { "Pequeno" => "Small", "Grande" => "Large", _ => "Medium" };
    }

    // ── Color Picker ──────────────────────────────────────────────────────────
    private async void OnColorPickerTap(object sender, EventArgs e)
    {
        string[] colors = { "Azul Profundo", "Azul Oceano", "Azul Noturno",
                            "Azul Escuro", "Azul Cinza", "Cinza Petróleo",
                            "Preto Azulado", "Grafite" };
        string[] hex    = { "#1B2A3B", "#0B4F6C", "#154360",
                            "#1A5276", "#2E4057", "#2C3E50",
                            "#17202A", "#212F3D" };

        string result = await DisplayActionSheet(
            "Cor da nota", "Cancelar", null, colors);

        int idx = Array.IndexOf(colors, result);
        if (idx >= 0) _vm.NoteColor = hex[idx];
    }
}
