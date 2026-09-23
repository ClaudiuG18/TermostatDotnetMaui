using TermostatAndroid.Model;
using TermostatAndroid.ViewModel;

namespace TermostatAndroid.View;

public partial class DetailsPage : ContentPage, IQueryAttributable
{
    private DetailsPageViewModel? _viewModel;

    private Task? _pollingTask;

    public DetailsPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(
     IDictionary<string, object> query)
    {
        if (!query.TryGetValue(
                "EspData",
                out var dataValue))
            return;

        if (dataValue is not EspData espData)
            return;

        if (!query.TryGetValue(
                "IpAddress",
                out var ipValue))
            return;

        if (ipValue is not string ipAddress)
            return;

        _viewModel =
            new DetailsPageViewModel(
                espData,
                ipAddress
            );

        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel != null)
        {
            _pollingTask =
                _viewModel.StartPolling();
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        _viewModel?.StopPolling();
    }
}