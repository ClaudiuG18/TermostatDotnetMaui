using TermostatAndroid.Model;
using TermostatAndroid.Service;

namespace TermostatAndroid.View;

public partial class SettingsPage : ContentPage
{
    private readonly EspConfigService _espConfigService;

    public SettingsPage()
    {
        InitializeComponent();

        _espConfigService = new EspConfigService();

        LoadEspList();
    }


    // =====================================================
    // ÎNCARCĂ LISTA
    // =====================================================

    private void LoadEspList()
    {
        EspListLayout.Children.Clear();

        var configs = _espConfigService.GetAll();

        foreach (var esp in configs)
        {
            AddEspCard(esp);
        }
    }


    // =====================================================
    // CARD ESP
    // =====================================================

    private void AddEspCard(EspConfig esp)
    {
        var nameLabel = new Label
        {
            Text = esp.Name,
            FontSize = 22,
            FontAttributes = FontAttributes.Bold
        };

        var ipLabel = new Label
        {
            Text = $"IP: {esp.IpAddress}",
            FontSize = 17
        };

        var editButton = new Button
        {
            Text = "Editează"
        };

        var deleteButton = new Button
        {
            Text = "Șterge"
        };

        var buttonsLayout = new HorizontalStackLayout
        {
            Spacing = 10,
            Children =
            {
                editButton,
                deleteButton
            }
        };

        var layout = new VerticalStackLayout
        {
            Spacing = 8,
            Children =
            {
                nameLabel,
                ipLabel,
                buttonsLayout
            }
        };

        var border = new Border
        {
            Padding = 15,
            Content = layout
        };

        EspListLayout.Children.Add(border);


        // =================================================
        // EDITARE
        // =================================================

        editButton.Clicked += async (sender, args) =>
        {
            await EditEsp(esp);
        };


        // =================================================
        // ȘTERGERE
        // =================================================

        deleteButton.Clicked += async (sender, args) =>
        {
            await DeleteEsp(esp);
        };
    }


    // =====================================================
    // ADAUGĂ ESP
    // =====================================================

    private async void OnAddEspClicked(
        object sender,
        EventArgs e)
    {
        string? name = await DisplayPromptAsync(
            "Adaugă ESP",
            "Numele camerei:",
            placeholder: "Ex: Living"
        );

        if (string.IsNullOrWhiteSpace(name))
            return;


        string? ip = await DisplayPromptAsync(
            "Adaugă ESP",
            "Adresa IP:",
            placeholder: "Ex: 192.168.2.201"
        );

        if (string.IsNullOrWhiteSpace(ip))
            return;


        if (!_espConfigService.AddEsp(
                name,
                ip,
                out string errorMessage))
        {
            await DisplayAlert(
                "Eroare",
                errorMessage,
                "OK"
            );

            return;
        }


        LoadEspList();
    }


    // =====================================================
    // EDITARE ESP
    // =====================================================

    private async Task EditEsp(EspConfig esp)
    {
        string? name = await DisplayPromptAsync(
            "Editează ESP",
            "Numele camerei:",
            initialValue: esp.Name
        );

        if (string.IsNullOrWhiteSpace(name))
            return;


        string? ip = await DisplayPromptAsync(
            "Editează ESP",
            "Adresa IP:",
            initialValue: esp.IpAddress
        );

        if (string.IsNullOrWhiteSpace(ip))
            return;


        if (!_espConfigService.IsValidIp(ip))
        {
            await DisplayAlert(
                "Eroare",
                "Adresa IP nu este validă.",
                "OK"
            );

            return;
        }


        var configs = _espConfigService.GetAll();

        // Verificăm dacă IP-ul este folosit
        // de ALT ESP.
        bool duplicate = configs.Any(x =>
            x.Id != esp.Id &&
            x.IpAddress.Equals(
                ip.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (duplicate)
        {
            await DisplayAlert(
                "Eroare",
                "Acest IP este deja folosit.",
                "OK"
            );

            return;
        }


        esp.Name = name.Trim();
        esp.IpAddress = ip.Trim();

        _espConfigService.SaveAll(configs);

        LoadEspList();
    }


    // =====================================================
    // ȘTERGERE ESP
    // =====================================================

    private async Task DeleteEsp(EspConfig esp)
    {
        bool confirm = await DisplayAlert(
            "Ștergere",
            $"Ștergi ESP-ul „{esp.Name}”?",
            "Da",
            "Nu"
        );

        if (!confirm)
            return;


        var configs = _espConfigService.GetAll();

        configs.RemoveAll(x =>
            x.Id == esp.Id);

        _espConfigService.SaveAll(configs);

        LoadEspList();
    }
}