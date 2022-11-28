using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CurseForgeDownloader.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected IObservable<bool> IsBusyObservable { get; init; }

        private string? _statusText;
        public string? StatusText
        {
            get => _statusText;
            set => Dispatcher.UIThread.Post(() => this.RaiseAndSetIfChanged(ref _statusText, value));
        }
        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => Dispatcher.UIThread.Post(() => this.RaiseAndSetIfChanged(ref _isBusy, value));
        }
        public bool HasError { get; set; }


        protected ViewModelBase()
        {
            IsBusyObservable = this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy);
            IsBusyObservable.Subscribe((isBusy) =>
            {
                if (isBusy)
                    StatusText = null;
            });
        }
    }
}
