using System.IO;
using JetBrains.Annotations;

namespace RevitLookup.Common.Extensions;

[PublicAPI]
public static class SystemExtensions
{
    extension(string source)
    {
        /// <summary>
        ///     Combines strings into a path
        /// </summary>
        /// <returns>
        ///     The combined paths.
        ///     If one of the specified paths is a zero-length string, this method returns the other path.
        ///     If path2 contains an absolute path, this method returns path2.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     NET Framework and .NET Core versions older than 2.1: path1 or path2 contains one or more of the invalid characters defined in <see cref="Path.GetInvalidPathChars" />
        /// </exception>
        /// <exception cref="System.ArgumentNullException">source or path is null</exception>
        [Pure]
        public string AppendPath(string path)
        {
            return Path.Combine(source, path);
        }

        /// <summary>
        ///     Combines strings into a path
        /// </summary>
        /// <returns>The combined paths</returns>
        /// <exception cref="System.ArgumentException">
        ///     NET Framework and .NET Core versions older than 2.1: path1 or path2 contains one or more of the invalid characters defined in <see cref="Path.GetInvalidPathChars" />
        /// </exception>
        /// <exception cref="System.ArgumentNullException">source or path is null</exception>
        [Pure]
        public string AppendPath(params string[] paths)
        {
            var strings = new string[paths.Length + 1];
            strings[0] = source;
            for (var i = 1; i < strings.Length; i++)
            {
                strings[i] = paths[i - 1];
            }

            return Path.Combine(strings);
        }
    }

    extension(Type type)
    {
        /// <summary>
        ///     Determines whether the specified type is a primitive type.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if the type is a primitive type, an enumeration, or a string; otherwise, false.
        /// </returns>
        public bool IsPrimitiveType() => type.IsPrimitive || type.IsEnum || type == typeof(string);
    }
}