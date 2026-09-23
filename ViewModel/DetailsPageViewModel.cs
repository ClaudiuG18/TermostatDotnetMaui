using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TermostatAndroid.Model;
using TermostatAndroid.Service;

namespace TermostatAndroid.ViewModel;

public class DetailsPageViewModel : INotifyPropertyChanged
{
    private readonly EspService _espService;
    private CancellationTokenSource? _pollingCts;
    private string _id = "";
    private readonly string _ipAddress;

    private double _temp;
    private double _setpoint;
    private double _calibTemp;
    private double _hyst;
   

    private int _relay;

    private string _statusMessage = "";


    // ==========================================
    // PROPERTIES
    // ==========================================

    public string Id
    {
        get => _id;
        set
        {
            _id = value;
            OnPropertyChanged();
        }
    }


    public double Temp
    {
        get => _temp;
        set
        {
            _temp = value;
            OnPropertyChanged();
        }
    }


    public double Setpoint
    {
        get => _setpoint;
        set
        {
            _setpoint =
            Math.Round(
                value,
                1,
                MidpointRounding.AwayFromZero
            );
            OnPropertyChanged();
        }
    }


    public double CalibTemp
    {
        get => _calibTemp;
        set
        {
            _calibTemp =
            Math.Round(
                value,
                1,
                MidpointRounding.AwayFromZero
            );
            OnPropertyChanged();
        }
    }


    public double Hyst
    {
        get => _hyst;
        set
        {
            _hyst =
            Math.Round(
                value,
                1,
                MidpointRounding.AwayFromZero
            );
            OnPropertyChanged();
        }
    }


    public int Relay
    {
        get => _relay;
        set
        {
            _relay = value;

            OnPropertyChanged();
            OnPropertyChanged(nameof(RelayText));
        }
    }


    public string RelayText =>
        Relay == 1
            ? "🔥 Încălzire PORNITĂ"
            : "Încălzire OPRITĂ";


    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            _statusMessage = value;
            OnPropertyChanged();
        }
    }


    // ==========================================
    // COMMAND
    // ==========================================

    public ICommand SaveCommand { get; }


    // ==========================================
    // CONSTRUCTOR
    // ==========================================

    public DetailsPageViewModel(
    EspData espData,
    string ipAddress)
    {
        _espService = new EspService();

        _ipAddress = ipAddress;

        Id = espData.Id;
        Temp = espData.Temp;
        Setpoint = espData.Setpoint;
        CalibTemp = espData.CalibTemp;
        Hyst = espData.Hyst;
        Relay = espData.Relay;

        SaveCommand = new Command(
            async () => await Save()
        );
    }


    // ==========================================
    // SAVE
    // ==========================================

    private async Task Save()
    {
        try
        {
            StatusMessage = "Se salvează...";

            Console.WriteLine("========== SAVE ==========");

            Console.WriteLine(
                $"ID: {Id}"
            );

            Console.WriteLine(
                $"Setpoint: {Setpoint}"
            );

            Console.WriteLine(
                $"CalibTemp: {CalibTemp}"
            );

            Console.WriteLine(
                $"Hyst: {Hyst}"
            );


            // ==========================================
            // POST -> ESP
            // ==========================================

            bool success =
                await _espService.SetControl(
                    _ipAddress,
                    Setpoint,
                    CalibTemp,
                    Hyst
                );

            Console.WriteLine(
                $"SetControl result: {success}"
            );


            if (!success)
            {
                StatusMessage =
                    "ESP nu a acceptat comanda.";

                return;
            }


            // ==========================================
            // POST SUCCESS
            // ==========================================

            StatusMessage =
                "Setările au fost salvate.";

            Console.WriteLine(
                "POST către ESP reușit."
            );


            // ==========================================
            // GET -> ESP
            // verificăm valorile reale
            // ==========================================

            var data =
                await _espService.GetEspData(_ipAddress);

            Console.WriteLine(
                "GET după POST executat."
            );


            if (data != null)
            {
                Temp = data.Temp;

                Setpoint = data.Setpoint;

                CalibTemp = data.CalibTemp;

                Hyst = data.Hyst;

                Relay = data.Relay;

                Console.WriteLine(
                    $"ESP confirmă Setpoint: {data.Setpoint}"
                );

                Console.WriteLine(
                    $"ESP confirmă CalibTemp: {data.CalibTemp}"
                );

                Console.WriteLine(
                    $"ESP confirmă Relay: {data.Relay}"
                );
            }

            Console.WriteLine(
                "========== SAVE END =========="
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                "========== SAVE ERROR =========="
            );

            Console.WriteLine(ex.ToString());

            Console.WriteLine(
                "================================="
            );

            StatusMessage =
                $"Eroare: {ex.Message}";
        }
    }


    // ==========================================
    // PROPERTY CHANGED
    // ==========================================

    public event PropertyChangedEventHandler?
        PropertyChanged;


    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName)
        );
    }

    public async Task StartPolling()
    {
        if (_pollingCts != null)
            return;

        _pollingCts = new CancellationTokenSource();

        try
        {
            using var timer = new PeriodicTimer(
                TimeSpan.FromSeconds(1));

            while (await timer.WaitForNextTickAsync(
                _pollingCts.Token))
            {
                await UpdateFromEsp();
            }
        }
        catch (OperationCanceledException)
        {
            // Polling oprit normal
        }
        finally
        {
            _pollingCts.Dispose();
            _pollingCts = null;
        }
    }

    public void StopPolling()
    {
        _pollingCts?.Cancel();
    }
    private async Task UpdateFromEsp()
    {
        try
        {
            var data = await _espService.GetEspData(_ipAddress);

            if (data == null)
                return;

            Temp = data.Temp;
            Relay = data.Relay;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"DetailsPage polling error: {ex.Message}");
        }
    }
}