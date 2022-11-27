using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CurseForgeDownloader.ViewModels
{
    public class ViewModelBase : ReactiveObject
    {
        protected IObservable<bool> IsBusyObservable { get; init; } = null!;
        [Reactive]
        public bool IsBusy { get; set; }
        public bool HasError { get; set; }

        protected ViewModelBase()
        {
            this.WhenAnyValue(vm => vm.IsBusy, isBusy => !isBusy);
        }
    }
}
