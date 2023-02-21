using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using Mediator;


namespace CurseForgeDownloader.Messages;

public class CurseForgeErrorMessage : INotification
{
    public Exception[] Exceptions { get; init; }
    public string? Caller { get; init; }
    public CurseForgeErrorMessage([CallerMemberName] string? callsite = null, params Exception[] exceptions)
    {
        Exceptions = exceptions;
        Caller = callsite;
    }
}