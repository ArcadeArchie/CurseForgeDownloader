using Avalonia.Notification;
using Avalonia.Threading;
using CurseForgeDownloader.Messages;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Threading;
using Mediator;

namespace CurseForgeDownloader.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected IObservable<bool> IsBusyObservable { get; init; }

        // private string? _statusText;
        // public string? StatusText
        // {
        //     get => _statusText;
        //     set => Dispatcher.UIThread.Post(() => this.RaiseAndSetIfChanged(ref _statusText, value));
        // }
        // private bool _isBusy;
        // public bool IsBusy
        // {
        //     get => _isBusy;
        //     set => Dispatcher.UIThread.Post(() => this.RaiseAndSetIfChanged(ref _isBusy, value));
        // }
        [Reactive]
        public string? StatusText { get; set; }
        [Reactive]
        public bool IsBusy { get; set; }
        public bool HasError { get; set; }        
        public INotificationMessageManager Notifications { get; } = new NotificationMessageManager();

        protected ViewModelBase()
        {
            IsBusyObservable = this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy).ObserveOn(RxApp.MainThreadScheduler);
            IsBusyObservable.Subscribe((isBusy) =>
            {
                if (isBusy)
                    StatusText = null;
            });
        }


        protected void CreateErrorNotification(IList<string> errors)
        {
            var msg = Notifications
                        .CreateMessage()
                        .Accent("#f55b65")
                        .Animates(true)
                        .Background("#727272")
                        .HasBadge("Error");
            foreach (var error in errors) { msg.HasMessage(error); }
            msg
                .Dismiss().WithButton("Close", btn => { })
                .Dismiss().WithDelay(TimeSpan.FromSeconds(10));
            Dispatcher.UIThread.Post(() => msg.Queue(), DispatcherPriority.Normal);
        }
    }
}
