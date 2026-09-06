using System.IO;

namespace RevitLookup.UI.Playground.Extensions;

/// <summary>
///     Provides extension methods for <see cref="object" /> and <see cref="string" />.
/// </summary>
public static class SystemExtensions
{
    /// <param name="obj">The object to cast.</param>
    extension(object obj)
    {
        /// <summary>
        ///     Casts the object to the specified <typeparamref name="T" /> type.
        /// </summary>
        /// <typeparam name="T">The type to cast the object to.</typeparam>
        /// <returns>The object cast to <typeparamref name="T" />.</returns>
        [Pure]
        public T Cast<T>()
        {
            return (T)obj;
        }
    }

    /// <param name="source">The leading path to combine.</param>
    extension(string source)
    {
        /// <summary>
        ///     Combines <paramref name="source" /> and <paramref name="path" /> into a single path.
        /// </summary>
        /// <param name="path">The path to append to <paramref name="source" />.</param>
        /// <returns>
        ///     The combined path.
        ///     If <paramref name="source" /> or <paramref name="path" /> is a zero-length string, this method returns the other path.
        ///     If <paramref name="path" /> contains an absolute path, this method returns <paramref name="path" />.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///     .NET Framework and .NET Core versions older than 2.1: <paramref name="source" /> or <paramref name="path" /> contains one or more of the invalid characters defined in <see cref="Path.GetInvalidPathChars" />.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="path" /> is <see langword="null" />.</exception>
        [Pure]
        public string AppendPath(string path)
        {
            return Path.Combine(source, path);
        }
    }
}
