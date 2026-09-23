using System.ComponentModel;
using System.Runtime.CompilerServices;
using TermostatAndroid.Model;

namespace TermostatAndroid.ViewModel;

public class EspCardViewModel : INotifyPropertyChanged
{
    private int _failedRequests = 0;

    private const int MaxFailedRequests = 3;
    private readonly EspConfig _config;

    private EspData? _data;

    private bool _isOnline;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string Id => _config.Id;

    public string Name => _config.Name;

    public string IpAddress => _config.IpAddress;

    public bool IsOnline
    {
        get => _isOnline;
        set
        {
            if (_isOnline == value)
                return;

            _isOnline = value;
            OnPropertyChanged();
        }
    }

    public double Temp =>
        _data?.Temp ?? 0;

    public double Hum =>
        _data?.Hum ?? 0;

    public double Setpoint =>
        _data?.Setpoint ?? 0;

    public double CalibTemp =>
        _data?.CalibTemp ?? 0;

    public int Relay =>
        _data?.Relay ?? 0;

    public EspData? CurrentEspData =>
        _data;

    public EspCardViewModel(EspConfig config)
    {
        _config = config;
    }

    public void UpdateData(EspData data)
    {
        _failedRequests = 0;

        _data = data;

        IsOnline = true;

        OnPropertyChanged(nameof(Temp));
        OnPropertyChanged(nameof(Hum));
        OnPropertyChanged(nameof(Setpoint));
        OnPropertyChanged(nameof(CalibTemp));
        OnPropertyChanged(nameof(Relay));
        OnPropertyChanged(nameof(CurrentEspData));
    }

   

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }

    public void RequestFailed()
    {
        _failedRequests++;

        Console.WriteLine(
            $"{Name}: request failed " +
            $"{_failedRequests}/{MaxFailedRequests}"
        );

        if (_failedRequests >= MaxFailedRequests)
        {
            IsOnline = false;

            _data = null;

            OnPropertyChanged(nameof(CurrentEspData));
            OnPropertyChanged(nameof(Temp));
            OnPropertyChanged(nameof(Hum));
            OnPropertyChanged(nameof(Setpoint));
            OnPropertyChanged(nameof(CalibTemp));
            OnPropertyChanged(nameof(Relay));
        }
    }
}