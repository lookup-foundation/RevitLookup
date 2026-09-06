namespace RevitLookup.UI.Framework.Extensions;

/// <summary>
///     Provides extension methods for <see cref="IEnumerable{T}" /> to pick or reorder its elements at random.
/// </summary>
[PublicAPI]
public static class EnumerableExtensions
{
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="collection">The collection to pick from or reorder.</param>
    extension<T>(IEnumerable<T> collection)
    {
        /// <summary>
        ///     Returns a random element from the collection.
        /// </summary>
        /// <returns>A randomly selected element from the collection.</returns>
        /// <exception cref="InvalidOperationException">The collection contains no elements.</exception>
        public T Random()
        {
            if (collection is not IList<T> list)
            {
                list = collection.ToArray();
            }

            if (list.Count == 0)
            {
                throw new InvalidOperationException("Collection contains no elements");
            }

            return list[System.Random.Shared.Next(list.Count)];
        }

        /// <summary>
        ///     Returns the collection elements reordered at random.
        /// </summary>
        /// <returns>A <see cref="List{T}" /> containing the elements of the collection in random order.</returns>
        /// <remarks>
        ///     When the collection is already a <see cref="List{T}" />, this method shuffles it in place and returns that same instance.
        /// </remarks>
        public List<T> Randomize()
        {
            if (collection is not List<T> list)
            {
                list = collection.ToList();
            }

            var count = list.Count;
            while (count > 1)
            {
                count--;
                var k = System.Random.Shared.Next(count + 1);
                (list[k], list[count]) = (list[count], list[k]);
            }

            return list;
        }
    }
}
