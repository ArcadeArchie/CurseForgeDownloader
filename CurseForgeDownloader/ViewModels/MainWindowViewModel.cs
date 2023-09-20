using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CurseForgeDownloader.Messages;
using CurseForgeDownloader.Models;
using CurseForgeDownloader.Services;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CurseForgeDownloader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, Mediator.INotificationHandler<CurseForgeErrorMessage>
    {
        private static readonly FilePickerOpenOptions FilePicker = new()
        {
            Title = "Select Manifest JSON or Modpack zip",
            AllowMultiple = false,

            FileTypeFilter = new List<FilePickerFileType>
            {
                new("CurseForge modepack files")
                {
                    Patterns = new [] { "*.json",  "*.zip"  },
                    MimeTypes = new[] { "application/json", "application/zip" }
                }
            }
        };
        private static readonly FolderPickerOpenOptions FolderPicker = new()
        {
            Title = "Select Output directory",
            AllowMultiple = false
        };
        private readonly CurseForgeManifestService? _manifestService;
        private readonly IOptionsMonitor<Config.AppConfig>? _config;
        private readonly DebounceDispatcher _debounce = new();

        #region Properties

        [Reactive]
        public string? ApiKey { get; set; }
        [Reactive]
        public string? ManifestPath { get; set; }
        [Reactive]
        public CurseForgeManifest? CurrentManifest { get; set; }
        [Reactive]
        public bool HasManifest { get; set; }
        public ReactiveCommand<Window, Unit>? SelectManifestCmd { get; private set; }
        public ReactiveCommand<Window, Unit>? ExtractModsCmd { get; private set; }
        public ReactiveCommand<Window, Unit>? CreatePackFolderCmd { get; private set; }

        #endregion

        public MainWindowViewModel()
        {
            _manifestService = Program.AppHost?.Services.GetRequiredService<CurseForgeManifestService>();
            _config = Program.AppHost?.Services.GetRequiredService<IOptionsMonitor<Config.AppConfig>>();
            ApiKey = _config?.CurrentValue.ApiKey;
            CreateCommands();

            this.WhenAnyValue(x => x.ManifestPath).Subscribe(async x => await ProcessManifest(x));
            this.WhenAnyValue(x => x.CurrentManifest).Subscribe(x => HasManifest = x != null);

            this.WhenPropertyChanged(x => x.ApiKey).Subscribe(x =>
            {
                //wait a bit before commiting to config
                _debounce.Debounce(1500, e =>
                {
                    if (string.IsNullOrEmpty(e) || _config?.CurrentValue.ApiKey == e)
                        return;
                    _config!.CurrentValue.ApiKey = e;
                }, ApiKey);
            });
        }

        private void CreateCommands()
        {
            var requireApiKey = Observable.CombineLatest(IsBusyObservable, this.WhenAnyValue(x => x.ApiKey), (isBusy, key) => isBusy && !string.IsNullOrEmpty(key));
            var requireKeyAndFromZip = Observable.CombineLatest(requireApiKey, this.WhenAnyValue(x => x.CurrentManifest!.FromZip), (isBusy, isZip) => isBusy && isZip);

            SelectManifestCmd = ReactiveCommand.CreateFromTask<Window>(HandleSelectCmd, IsBusyObservable);
            ExtractModsCmd = ReactiveCommand.CreateFromTask<Window>(HandleExtractModsCmd, requireApiKey);
            CreatePackFolderCmd = ReactiveCommand.CreateFromTask<Window>(HandleCreatePackFolderCmd, requireKeyAndFromZip);
        }

        private async Task HandleExtractModsCmd(Window parent)
        {
            IsBusy = true;
            StatusText = "Extracting Mods";
            var res = await parent.StorageProvider.OpenFolderPickerAsync(FolderPicker);
            if (res is not null && res.Any())
            {
                var folder = res[0];
                if (folder is not null)
                {
                    await _manifestService!.ExtractMods(CurrentManifest!, folder.Path.AbsolutePath);
                }
            }
            IsBusy = false;
        }

        private async Task HandleCreatePackFolderCmd(Window parent)
        {
            IsBusy = true;
            StatusText = "Creating Modpack folder";
            var res = await parent.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "Select a Extract location"
            });
            if (res is not null && res.Any())
            {
                var file = res[0];
                if (file is not null)
                {
                    await _manifestService!.CreatePack(CurrentManifest!, file.Path.AbsolutePath);
                }
            }
            IsBusy = false;
        }

        private async Task ProcessManifest(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;
            CurrentManifest = null;
            IsBusy = true;
            StatusText = "Proccessing Manifest";
            CurrentManifest = await _manifestService!.RetrieveManifest(path);
            IsBusy = false;
        }

        private async Task HandleSelectCmd(Window parent)
        {
            IsBusy = true;
            StatusText = "Choosing Manifest";
            var res = await parent.StorageProvider.OpenFilePickerAsync(FilePicker);
            IsBusy = false;

            if (res is null || !res.Any())
                return;
            var file = res[0];
            if (file is null)
                return;
            ManifestPath = file.Path.AbsolutePath;
        }



        public ValueTask Handle(CurseForgeErrorMessage notification, CancellationToken cancellationToken)
        {
            CreateErrorNotification(notification.Exceptions.Select(x => x.Message).ToArray());
            return ValueTask.CompletedTask;
        }
    }
}
