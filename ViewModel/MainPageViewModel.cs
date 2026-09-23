using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TermostatAndroid.Service;

namespace TermostatAndroid.ViewModel;

public class MainPageViewModel : INotifyPropertyChanged
{
    private readonly EspService _espService;
    private readonly EspConfigService _configService;

    private CancellationTokenSource? _pollingCts;

    public ObservableCollection<EspCardViewModel> EspCards { get; }
        = new();

    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand RefreshCommand { get; }

    public MainPageViewModel()
    {
        _espService = new EspService();
        _configService = new EspConfigService();

        RefreshCommand = new Command(
            LoadEspConfigs
        );

        LoadEspConfigs();
    }

    // =====================================================
    // ÎNCARCĂ ESP-URILE
    // =====================================================

    private void LoadEspConfigs()
    {
        EspCards.Clear();

        var configs = _configService.GetAll();

        foreach (var config in configs)
        {
            EspCards.Add(
                new EspCardViewModel(config)
            );
        }
    }

    // =====================================================
    // POLLING
    // =====================================================

    public async Task StartPolling()
    {
        if (_pollingCts != null)
            return;

        _pollingCts =
            new CancellationTokenSource();

        try
        {
            while (
                !_pollingCts.Token.IsCancellationRequested)
            {
                foreach (var card in EspCards.ToList())
                {
                    await UpdateEsp(card);
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    _pollingCts.Token
                );
            }
        }
        catch (OperationCanceledException)
        {
            // Oprire normală
        }
        finally
        {
            _pollingCts.Dispose();
            _pollingCts = null;
        }
    }

    // =====================================================
    // UPDATE UN ESP
    // =====================================================

    private async Task UpdateEsp(
        EspCardViewModel card)
    {
        var data =
            await _espService.GetEspData(
                card.IpAddress
            );

        if (data != null)
        {
            card.UpdateData(data);
        }
        else
        {
            card.RequestFailed();
        }
    }

    // =====================================================
    // STOP POLLING
    // =====================================================

    public void StopPolling()
    {
        _pollingCts?.Cancel();
    }

    // =====================================================
    // PROPERTY CHANGED
    // =====================================================

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName)
        );
    }
}