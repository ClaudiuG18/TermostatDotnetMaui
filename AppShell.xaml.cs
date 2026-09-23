using TermostatAndroid.View;

namespace TermostatAndroid;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(
            nameof(DetailsPage),
            typeof(DetailsPage)
        );
        Routing.RegisterRoute(
            nameof(SettingsPage),
            typeof(SettingsPage)
        );
    }
}