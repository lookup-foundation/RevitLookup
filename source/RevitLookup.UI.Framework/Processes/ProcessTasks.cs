using System.Diagnostics;
using System.Text;

namespace RevitLookup.UI.Framework.Processes;

/// <summary>
///     Tasks for starting and managing processes. Supports different APIs for .NET Core and .NET Framework.
/// </summary>
public static class ProcessTasks
{
    /// <summary>
    ///     Starts a process and redirects its standard output and error to the specified logger.
    /// </summary>
    /// <param name="toolPath">The path of the executable to start.</param>
    /// <param name="arguments">The command-line arguments to pass to the process.</param>
    /// <param name="logger">The callback invoked for each output line. When <see langword="null" />, output is written to the console.</param>
    /// <returns>The started <see cref="Process" />, or <see langword="null" /> if the process could not be started.</returns>
    public static Process? StartProcess(string toolPath, string arguments = "", Action<OutputType, string>? logger = null)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = toolPath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8,
            StandardErrorEncoding = Encoding.UTF8
        };

        var process = Process.Start(startInfo);
        if (process == null)
        {
            return null;
        }

        RedirectProcessOutput(process, logger);
        return process;
    }

    /// <summary>
    ///     Starts a process through the shell, using the shell's file associations to open <paramref name="toolPath" />.
    /// </summary>
    /// <param name="toolPath">The path or URI to open.</param>
    /// <param name="arguments">The command-line arguments to pass to the process.</param>
    /// <returns>The started <see cref="Process" />, or <see langword="null" /> if the process could not be started.</returns>
    public static Process? StartShell(string toolPath, string arguments = "")
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = toolPath,
            Arguments = arguments,
            CreateNoWindow = true,
            UseShellExecute = true
        };

        return Process.Start(startInfo);
    }

    /// <summary>
    ///     Redirect the process output to the logger
    /// </summary>
    private static void RedirectProcessOutput(Process process, Action<OutputType, string>? logger)
    {
        logger ??= DefaultLogger;
        process.OutputDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            logger.Invoke(OutputType.Standard, args.Data);
        };
        process.ErrorDataReceived += (_, args) =>
        {
            if (string.IsNullOrEmpty(args.Data))
            {
                return;
            }

            logger.Invoke(OutputType.Error, args.Data);
        };

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }

    /// <summary>
    ///     Default logger for the process output
    /// </summary>
    private static void DefaultLogger(OutputType type, string output)
    {
        Console.WriteLine(output);
    }
}

/// <summary>
///     Determines the stream a process output line came from.
/// </summary>
public enum OutputType
{
    /// <summary>
    ///     The line came from the process's standard output stream.
    /// </summary>
    Standard,

    /// <summary>
    ///     The line came from the process's standard error stream.
    /// </summary>
    Error
}
