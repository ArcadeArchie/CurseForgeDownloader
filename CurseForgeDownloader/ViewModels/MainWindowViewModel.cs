using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.X11;
using CurseForgeDownloader.Models;
using CurseForgeDownloader.Services;
using DynamicData.Binding;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurseForgeDownloader.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private static readonly OpenFileDialog FilePicker = new OpenFileDialog
        {
            Title = "Select Manifest JSON or Modpack zip",
            AllowMultiple = false,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter
                {
                    Extensions = new List<string> { "json", "zip" }
                },
            }
        };
        private static readonly OpenFolderDialog FolderPicker = new OpenFolderDialog
        {
            Title = "Select Output directory"
        };
        private readonly CurseForgeManifestService? _manifestService;

        [Reactive]
        public string? ManifestPath { get; set; }
        [Reactive]
        public CurseForgeManifest? CurrentManifest { get; set; }
        [Reactive]
        public bool HasManifest { get; set; }
        public ReactiveCommand<Window, Unit> SelectManifestCmd { get; init; }
        public ReactiveCommand<Window, Unit> ExtractModsCmd { get; init; }
        public ReactiveCommand<Window, Unit> CreatePackFolderCmd { get; init; }

        public MainWindowViewModel()
        {
            _manifestService = Program.AppHost?.Services.GetRequiredService<CurseForgeManifestService>();
            SelectManifestCmd = ReactiveCommand.CreateFromTask<Window>(HandleSelectCmd, IsBusyObservable);
            ExtractModsCmd = ReactiveCommand.CreateFromTask<Window>(HandleExtractModsCmd, IsBusyObservable);
            CreatePackFolderCmd = ReactiveCommand.CreateFromTask<Window>(HandleCreatePackFolderCmd, 
                Observable.CombineLatest(
                    IsBusyObservable, 
                    this.WhenAny(x => x.CurrentManifest!.FromZip, x => x.Value), 
                    (isBusy, isZip) => isBusy && isZip));
            
            this.WhenAnyValue(x => x.ManifestPath).Subscribe(async x => await ProcessManifest(x));
            this.WhenAnyValue(x => x.CurrentManifest).Subscribe(x => HasManifest = x != null);
        }

        private async Task HandleExtractModsCmd(Window parent)
        {
            IsBusy = true;
            StatusText = "Extracting Mods";
            var res = await FolderPicker.ShowAsync(parent);
            if (res != null)
            {
                await _manifestService!.ExtractMods(CurrentManifest!, res);
            }
            IsBusy = false;
        }

        private async Task HandleCreatePackFolderCmd(Window parent)
        {
            IsBusy = true;
            StatusText = "Creating Modpack folder";
            var res = await FolderPicker.ShowAsync(parent);
            if (res != null)
            {
                await _manifestService!.CreatePack(CurrentManifest!, res);
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
            var res = await FilePicker.ShowAsync(parent);
            IsBusy = false;
            if (res == null)
                return;
            ManifestPath = res[0];
        }



    }
}
