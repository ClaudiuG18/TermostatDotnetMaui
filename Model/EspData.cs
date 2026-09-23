namespace TermostatAndroid.Model;

public class EspData
{
    public string Id { get; set; } = "";

    public double Temp { get; set; }

    public double RawTemp { get; set; }

    public double Hum { get; set; }

    public double Setpoint { get; set; }

    public double CalibTemp { get; set; }

    public double Hyst { get; set; }

    public int Relay { get; set; }
}