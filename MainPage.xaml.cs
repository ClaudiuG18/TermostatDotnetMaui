using TermostatAndroid.Service;
using TermostatAndroid.Model;


namespace TermostatAndroid
{
    public partial class MainPage : ContentPage
    {
        private readonly EspService _espService;

        public MainPage()
        {
            InitializeComponent();
            _espService = new EspService();
        }
        private async void OnGetDataClicked(object sender, EventArgs e)
        {
            var response = await _espService.GetEspData();

            if (response == null)
                return;

            if (response.RawDataFromESP.TryGetValue("Test1", out EspData? esp))
            {
                IdLabel.Text = $"ID: {esp.id}";
                TempLabel.Text = $"Temperatură: {esp.temp:F2} °C";
                HumLabel.Text = $"Umiditate: {esp.hum} %";
                SetpointLabel.Text = $"Setpoint: {esp.setpoint} °C";
                RelayLabel.Text = $"Relay: {(esp.relay == 1 ? "ON" : "OFF")}";
            }


        }



    }
}
