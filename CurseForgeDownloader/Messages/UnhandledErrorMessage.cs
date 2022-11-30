using System;
using System.Runtime.CompilerServices;
using Mediator;


namespace CurseForgeDownloader.Messages;

public class UnhandledErrorMessage : INotification
{
    public Exception Exception { get; init; }
    public string? Caller { get; init; }
    public UnhandledErrorMessage(Exception exception, [CallerMemberName] string? callsite = null)
    {
        Exception = exception;
        Caller = callsite;
    }
}