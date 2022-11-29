using Avalonia;
using Avalonia.ReactiveUI;
using CurseForgeDownloader.Services;
using CurseForgeDownloader.ViewModels;
using CurseForgeDownloader.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace CurseForgeDownloader
{
    internal class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.

        internal static IHost? AppHost { get; private set; } = null!;

        [STAThread]
        public static async Task Main(string[] args)
        {
            AppHost = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostCtx, services) =>
                {

                    services.AddHttpClient("CurseApi", httpClient =>
                    {
                        httpClient.BaseAddress = new Uri(APIConstants.BaseURL);
                        httpClient.DefaultRequestHeaders.Add(APIConstants.APIKey, APIConstants.APIKeyValue);
                    });

                    services
                    .AddMediator()
                    .AddSingleton<CurseForgeManifestService>()
                    .AddSingleton<MainWindowViewModel>();

                    services.AddSingleton(BuildAvaloniaApp());
                })
                .Build();
            await AppHost.StartAsync();

            AppHost.Services.GetRequiredService<AppBuilder>().StartWithClassicDesktopLifetime(args);

            await AppHost.StopAsync();
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
