using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Json;
using TermostatAndroid.Model;

namespace TermostatAndroid.Service
{
   
    public class EspService
    {
        private readonly HttpClient _httpClient;
        public EspService()
        {
            _httpClient = new HttpClient();
        }
        public async Task<EspResponse?> GetEspData()
        {
            string url = "http://192.168.2.223:9999/api/rawDataFromESP";

            return await _httpClient.GetFromJsonAsync<EspResponse>(url);
        }
    }
}
