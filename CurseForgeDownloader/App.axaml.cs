using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Themes.Fluent;
using CurseForgeDownloader.Config;
using CurseForgeDownloader.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CurseForgeDownloader
{
    public partial class App : Application
    {
        private readonly Config.AppConfig _config;

        public App()
        {
            _config = Program.AppHost!.Services.GetRequiredService<IOptions<Config.AppConfig>>().Value;
        }
        public override void Initialize()
        {
            DataContext = _config.EnableDarkMode ? FluentThemeMode.Dark : FluentThemeMode.Light;
            if (_config.EnableDarkMode)
                Resources.Add("SystemControlBackgroundAltHighBrush", SolidColorBrush.Parse("#262626"));
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
