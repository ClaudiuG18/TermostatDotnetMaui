using TermostatAndroid.View;
using TermostatAndroid.ViewModel;

namespace TermostatAndroid;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _viewModel;

    private Task? _pollingTask;


    public MainPage()
    {
        InitializeComponent();

        _viewModel =
            new MainPageViewModel();

        BindingContext =
            _viewModel;
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();

        _pollingTask =
            _viewModel.StartPolling();
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel.StopPolling();
    }


    private async void OnDetailsClicked(
     object sender,
     EventArgs e)
    {
        if (sender is not Button button)
            return;

        if (button.BindingContext
            is not EspCardViewModel card)
            return;

        var espData = card.CurrentEspData;

        if (espData == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(DetailsPage),
            new Dictionary<string, object>
            {
                ["EspData"] = espData,
                ["IpAddress"] = card.IpAddress
            });
    }
    private async void OnSettingsClicked(
    object sender,
    EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(SettingsPage));
    }
}