using System.Collections.ObjectModel;

namespace RevitLookup.UI.Framework.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IEnumerable{T}" /> and <see cref="List{T}" /> to convert them into an <see cref="ObservableCollection{T}" />.
/// </summary>
public static class CollectionExtensions
{
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The <see cref="IEnumerable{T}" /> source to convert.</param>
    extension<T>(IEnumerable<T> source)
    {
        /// <summary>
        ///     Creates an <see cref="ObservableCollection{T}" /> from the source.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{T}" /> containing the elements of the source.</returns>
        public ObservableCollection<T> ToObservableCollection()
        {
            return new ObservableCollection<T>(source);
        }
    }

    /// <typeparam name="T">The type of elements in the list.</typeparam>
    /// <param name="source">The <see cref="List{T}" /> source to convert.</param>
    extension<T>(List<T> source)
    {
        /// <summary>
        ///     Creates an <see cref="ObservableCollection{T}" /> from the source.
        /// </summary>
        /// <returns>An <see cref="ObservableCollection{T}" /> containing the elements of the source.</returns>
        public ObservableCollection<T> ToObservableCollection()
        {
            return new ObservableCollection<T>(source);
        }
    }
}
