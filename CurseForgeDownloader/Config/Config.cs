

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CurseForgeDownloader.Config;

internal class AppConfig : INotifyPropertyChanged
{
    private bool _enableDarkMode;
    public bool EnableDarkMode
    {
        get => _enableDarkMode;
        set
        {
            _enableDarkMode = value;
            this.OnPropertyChanged(nameof(EnableDarkMode), value);
        }
    }
    private string? _apiKey;
    public string? ApiKey
    {
        get => _apiKey;
        set
        {
            _apiKey = value;
            this.OnPropertyChanged(nameof(ApiKey), value);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null, object? value = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgsEx(propertyName, value));
    }

}
public class PropertyChangedEventArgsEx : PropertyChangedEventArgs
{
    private object? _value;

    public virtual object? Value => _value;

    public PropertyChangedEventArgsEx(string? propertyName, object? value) : base(propertyName)
    {
        _value = value;
    }
}
