using System.Runtime.InteropServices;
using Autodesk.Revit.DB.ExtensibleStorage;

namespace RevitLookup.Utils;

[PublicAPI]
public static class SchemaExtensions
{
    /// <summary>
    ///     Begins a scope that grants unrestricted read access to the schema.
    ///     Access is automatically revoked when the returned scope is disposed.
    /// </summary>
    /// <param name="schema">The schema to grant access to.</param>
    /// <returns>A disposable scope. Call Dispose or use a 'using' statement to revoke access.</returns>
    /// <example>
    ///     <code>
    ///         using (schema.GrantAccess())
    ///         {
    ///             var entity = element.GetEntity(schema);
    ///         }
    ///     </code>
    /// </example>
    public static IDisposable GrantAccess(this Schema schema)
    {
        return SchemaAccessScope.Open();
    }
}

internal sealed
#if NET
    partial
#endif
    class SchemaAccessScope : IDisposable
{
    /// <summary>
    ///     ExtensibleStorage host module.
    /// </summary>
    private const string ModuleName = "Utility.dll";

    /// <summary>
    ///     Mangled name of the exported <c>s_checkAccess</c> global function pointer
    /// </summary>
    private const string ExportSymbol = "?s_checkAccess@@3P6A_NW4Enum@ESAccessLevel@@AEBVAString@@AEBVGUIDvalue@@@ZEA";

    /// <summary>
    ///     Win32 PAGE_READWRITE memory protection flag (PAGE_READWRITE = 0x04).
    /// </summary>
    private const uint PageReadWrite = 0x04;

    /// <summary>
    ///     Size, in bytes, of the patched pointer.
    /// </summary>
    private const int PointerSize = sizeof(long);

    private readonly nint _address;
    private readonly long _original;
    private int _disposed;

    private SchemaAccessScope(nint address, long original)
    {
        _address = address;
        _original = original;
    }

    internal static SchemaAccessScope Open()
    {
        var target = ResolveTarget();
        var original = Marshal.ReadInt64(target);

        Write(target, 0L);

        return new SchemaAccessScope(target, original);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        Write(_address, _original);
    }

    private static nint ResolveTarget()
    {
#if NET
        if (!NativeLibrary.TryLoad(ModuleName, out var module))
        {
            throw new SchemaAccessException($"Module '{ModuleName}' not found in current process.");
        }

        if (!NativeLibrary.TryGetExport(module, ExportSymbol, out var address))
        {
            throw new SchemaAccessException($"Export '{ExportSymbol}' not found in '{ModuleName}'.");
        }
#else
        var module = LoadLibrary(ModuleName);
        if (module == IntPtr.Zero)
        {
            throw new SchemaAccessException($"Module '{ModuleName}' not found in current process.");
        }

        var address = GetProcAddress(module, ExportSymbol);
        if (address == IntPtr.Zero)
        {
            throw new SchemaAccessException($"Export '{ExportSymbol}' not found in '{ModuleName}'.");
        }
#endif

        return address;
    }

    private static void Write(nint address, long value)
    {
        if (!VirtualProtect(address, PointerSize, PageReadWrite, out var oldProtect))
        {
            throw new SchemaAccessException($"VirtualProtect failed at 0x{address.ToInt64():X16}, error {Marshal.GetLastWin32Error()}.");
        }

        try
        {
            Marshal.WriteInt64(address, value);
        }
        finally
        {
            VirtualProtect(address, PointerSize, oldProtect, out _);
        }
    }

#if NET
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool VirtualProtect(nint address, nuint size, uint newProtect, out uint oldProtect);
#else
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool VirtualProtect(nint address, nuint size, uint newProtect, out uint oldProtect);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint LoadLibrary(string lpLibFileName);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    private static extern nint GetProcAddress(nint hModule, string lpProcName);
#endif
}

public sealed class SchemaAccessException(string message) : Exception(message);