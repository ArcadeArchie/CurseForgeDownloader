using Avalonia;
using Avalonia.ReactiveUI;
using CurseForgeDownloader.Messages;
using CurseForgeDownloader.Services;
using CurseForgeDownloader.ViewModels;
using Mediator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Text.Json;
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
                .ConfigureAppConfiguration((configBuilder) =>
                {
                    configBuilder.Sources.Clear();
                    configBuilder
                        .AddJsonFile("appsettings.json", false)
                        .SetFileLoadExceptionHandler(HandleConfigLoadErr);
                })
                .ConfigureServices((hostCtx, services) =>
                {

                    services.AddHttpClient("CurseApi", httpClient =>
                    {
                        httpClient.BaseAddress = new Uri(APIConstants.BaseURL);
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

        private static void HandleConfigLoadErr(FileLoadExceptionContext context)
        {
            if (context.Exception.GetType() != typeof(FileNotFoundException))
                return;
            context.Ignore = true;
            using var cfg = File.Create("appsettings.json");
            using var sr = new StreamWriter(cfg);
            sr.Write(JsonSerializer.Serialize(new { }));
        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToTrace()
                .UseReactiveUI();
    }
}
