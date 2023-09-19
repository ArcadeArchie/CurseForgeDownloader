using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
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
            if (!Design.IsDesignMode)
                _config = Program.AppHost!.Services.GetRequiredService<IOptions<Config.AppConfig>>().Value;
            else
                _config = new AppConfig
                {
                    EnableDarkMode = true,
                };
        }
        public override void Initialize()
        {
            RequestedThemeVariant = _config.EnableDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
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
