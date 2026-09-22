namespace TermostatAndroid
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

     

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SetTempLabel.Text = $"{e.NewValue:F1} °C";

        }
    }
}
