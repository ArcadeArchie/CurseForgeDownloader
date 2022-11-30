using System;
using Avalonia.Threading;

namespace CurseForgeDownloader;


internal class DebounceDispatcher
{
    private DispatcherTimer? timer;
    private DateTime timerStarted { get; set; } = DateTime.UtcNow.AddYears(-1);


    public void Debounce<TParam>(int interval, Action<TParam?> action,
        TParam? param = null,
        DispatcherPriority priority = DispatcherPriority.ApplicationIdle) where TParam : class
    {
        // kill pending timer and pending ticks
        timer?.Stop();
        timer = null;

        // timer is recreated for each event and effectively
        // resets the timeout. Action only fires after timeout has fully
        // elapsed without other events firing in between
        timer = new DispatcherTimer(TimeSpan.FromMilliseconds(interval), priority, (s, e) =>
        {
            if (timer == null)
                return;

            timer?.Stop();
            timer = null;
            action.Invoke(param);
        });

        timer.Start();
    }
}