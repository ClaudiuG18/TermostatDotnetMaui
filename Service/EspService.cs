using System.Net.Http.Json;
using System.Text.Json;
using TermostatAndroid.Model;

namespace TermostatAndroid.Service;

public class EspService
{
    private readonly HttpClient _httpClient;

    public EspService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(2)
        };
    }

    // =====================================================
    // GET DATA
    // =====================================================

    public async Task<EspData?> GetEspData(
        string ipAddress)
    {
        string url =
            $"http://{ipAddress}/api/data";

        try
        {
            return await _httpClient
                .GetFromJsonAsync<EspData>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"GET {url} ERROR: {ex.Message}"
            );

            return null;
        }
    }


    // =====================================================
    // SET CONTROL
    // =====================================================

    public async Task<bool> SetControl(
        string ipAddress,
        double setpoint,
        double calibTemp,
        double hyst)
    {
        string url =
            $"http://{ipAddress}/api/control";

        var command = new
        {
            setpoint,
            calibTemp,
            hyst
        };

        try
        {
            string json =
                JsonSerializer.Serialize(command);

            Console.WriteLine($"POST: {url}");
            Console.WriteLine($"JSON: {json}");

            using var content =
                new StringContent(
                    json,
                    System.Text.Encoding.UTF8,
                    "application/json"
                );

            var response =
                await _httpClient.PostAsync(
                    url,
                    content
                );

            string responseBody =
                await response.Content
                    .ReadAsStringAsync();

            Console.WriteLine(
                $"HTTP Status: {(int)response.StatusCode}"
            );

            Console.WriteLine(
                $"Response: {responseBody}"
            );

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"POST EXCEPTION: {ex}"
            );

            return false;
        }
    }
}