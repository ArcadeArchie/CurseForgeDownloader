using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Themes.Fluent;
using CurseForgeDownloader.ViewModels;
using CurseForgeDownloader.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CurseForgeDownloader
{
    public partial class App : Application
    {
        private readonly IConfiguration _config;

        public App()
        {
            _config = Program.AppHost!.Services.GetRequiredService<IConfiguration>();
        }
        public override void Initialize()
        {
            var useDarkMode = _config.GetValue<bool>("EnableDarkMode");
            DataContext = useDarkMode ? FluentThemeMode.Dark : FluentThemeMode.Light;
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
