using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Threading;
using CurseForgeDownloader.Config;
using CurseForgeDownloader.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CurseForgeDownloader
{
    public partial class App : Application
    {
        private readonly IOptionsMonitor<AppConfig>? _config;

        public App()
        {
            _config = Program.AppHost?.Services.GetRequiredService<IOptionsMonitor<AppConfig>>();
            _config?.OnChange((cfg, str) =>
            {
                Dispatcher.UIThread.Post(() =>
                    RequestedThemeVariant = _config!.CurrentValue.EnableDarkMode ? ThemeVariant.Dark : ThemeVariant.Light
                );
            });
        }
        public override void Initialize()
        {
            if (!Design.IsDesignMode)
            {
                RequestedThemeVariant = _config!.CurrentValue.EnableDarkMode ? ThemeVariant.Dark : ThemeVariant.Light;
                if (_config.CurrentValue.EnableDarkMode)
                    Resources.Add("SystemControlBackgroundAltHighBrush", SolidColorBrush.Parse("#262626"));
            }
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
