using System.Net;
using System.Text.Json;
using TermostatAndroid.Model;

namespace TermostatAndroid.Service;

public class EspConfigService
{
    private const string PreferencesKey = "EspConfigs";

    // =====================================================
    // GET TOATE ESP-URILE
    // =====================================================

    public List<EspConfig> GetAll()
    {
        string json = Preferences.Default.Get(
            PreferencesKey,
            ""
        );

        if (string.IsNullOrWhiteSpace(json))
            return new List<EspConfig>();

        try
        {
            return JsonSerializer.Deserialize<List<EspConfig>>(json)
                   ?? new List<EspConfig>();
        }
        catch
        {
            return new List<EspConfig>();
        }
    }


    // =====================================================
    // SALVEAZĂ TOATE ESP-URILE
    // =====================================================

    public void SaveAll(List<EspConfig> configs)
    {
        string json = JsonSerializer.Serialize(configs);

        Preferences.Default.Set(
            PreferencesKey,
            json
        );
    }


    // =====================================================
    // VALIDARE IP
    // =====================================================

    public bool IsValidIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
            return false;

        return IPAddress.TryParse(
            ip.Trim(),
            out _
        );
    }


    // =====================================================
    // ADAUGĂ ESP
    // =====================================================

    public bool AddEsp(
        string name,
        string ipAddress,
        out string errorMessage)
    {
        errorMessage = "";

        name = name.Trim();
        ipAddress = ipAddress.Trim();

        // Verificăm numele
        if (string.IsNullOrWhiteSpace(name))
        {
            errorMessage = "Introdu numele ESP-ului.";
            return false;
        }

        // Verificăm IP-ul
        if (!IsValidIp(ipAddress))
        {
            errorMessage =
                "Adresa IP nu este validă.";

            return false;
        }

        var configs = GetAll();

        // Verificăm dacă IP-ul există deja
        if (configs.Any(x =>
            x.IpAddress.Equals(
                ipAddress,
                StringComparison.OrdinalIgnoreCase)))
        {
            errorMessage =
                "Acest IP este deja configurat.";

            return false;
        }

        var esp = new EspConfig
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            IpAddress = ipAddress
        };

        configs.Add(esp);

        SaveAll(configs);

        return true;
    }
}