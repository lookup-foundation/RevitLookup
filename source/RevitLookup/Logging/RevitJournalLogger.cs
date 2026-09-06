using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Nice3point.Revit.Toolkit.External;

namespace RevitLookup.Logging;

/// <summary>
///     Writes the log records of a single category to the Revit journal, one comment per record.
/// </summary>
/// <remarks>
///     A record takes the <c>TOKEN { payload }</c> shape Revit writes its own <c>API_ERROR</c> and <c>API_SUCCESS</c> comments in.
///     Revit prepends the comment marker and the timestamp.
/// </remarks>
public sealed partial class RevitJournalLogger(string addinName, string categoryName) : ILogger
{
    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) where TState : notnull
    {
        return NullLogger.Instance.BeginScope(state);
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel != LogLevel.None;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
        {
            return;
        }

        var message = formatter(state, exception);
        var payload = exception is null ? message : $"{message} {exception}";
        var record = $"{addinName}_{logLevel.ToString().ToUpperInvariant()} {{ {categoryName}: {payload} }}";

        //A journal reads only lines opening with an apostrophe as comments. A line break splits the record into replayable journal commands
        WriteJournalCommentEvent.Raise(record.ReplaceLineEndings(" "));
    }

    [ExternalEvent(AllowDirectInvocation = true)]
    private static void WriteJournalComment(string comment)
    {
        RevitApiContext.Application.WriteJournalComment(comment, true);
    }
}
