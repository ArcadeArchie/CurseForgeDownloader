using Avalonia.Controls;
using Avalonia.ReactiveUI;
using CurseForgeDownloader.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace CurseForgeDownloader.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public MainWindow()
        {
            DataContext = Program.AppHost?.Services.GetRequiredService<MainWindowViewModel>();
            InitializeComponent();
        }
    }
}
