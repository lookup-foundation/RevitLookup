// Copyright (c) Lookup Foundation and Contributors
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// THIS PROGRAM IS PROVIDED "AS IS" AND WITH ALL FAULTS.
// NO IMPLIED WARRANTY OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE IS PROVIDED.
// THERE IS NO GUARANTEE THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.

using System.Reflection;

namespace RevitLookup.Tests.Coverage.Discovery;

/// <summary>
///     Reads the reflected surface a report renders.
/// </summary>
internal static class ReflectionExtensions
{
    /// <param name="assembly">The assembly to read.</param>
    extension(Assembly assembly)
    {
        /// <summary>
        ///     Reads every type the process resolves, dropping a type whose dependencies are absent.
        /// </summary>
        /// <returns>The types of the assembly the process can load.</returns>
        /// <remarks>
        ///     The Revit API assemblies reference native and optional managed modules a test host does not always load.
        /// </remarks>
        public IEnumerable<Type> GetLoadableTypes()
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                return exception.Types.OfType<Type>();
            }
        }
    }

    /// <param name="type">The type to name.</param>
    extension(Type type)
    {
        /// <summary>
        ///     Renders the short name of the type with its generic arguments.
        /// </summary>
        /// <returns>The short name of <paramref name="type" />, followed by its generic arguments in angle brackets when it is generic.</returns>
        public string FormatName()
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var arguments = string.Join(", ", type.GetGenericArguments().Select(argument => argument.FormatName()));

            return $"{type.FormatDeclarationName()}<{arguments}>";
        }

        /// <summary>
        ///     Renders the short name of the type without generic arity, the identifier a source file spells.
        /// </summary>
        /// <returns>The short name of <paramref name="type" />, with any backtick arity suffix removed.</returns>
        public string FormatDeclarationName()
        {
            var arityIndex = type.Name.IndexOf('`');

            return arityIndex < 0 ? type.Name : type.Name[..arityIndex];
        }
    }
}
