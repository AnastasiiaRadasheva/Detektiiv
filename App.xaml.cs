using DetektivGame.Views;
using Microsoft.Extensions.DependencyInjection;


namespace DetektivGame;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        MainPage = new NavigationPage(new StartPage())
        {
            BarBackgroundColor = Color.FromArgb("#16213E"),
            BarTextColor = Color.FromArgb("#C9A84C")
        };
    }
}
