using JetBrains.Annotations;

namespace RevitLookup.Common.Extensions;

[PublicAPI]
public static class EnumerableExtensions
{
    extension<T>(IEnumerable<T> collection)
    {
        public T Random()
        {
            if (collection is not IList<T> list)
            {
                list = collection.ToArray();
            }

            if (list.Count == 0) throw new InvalidOperationException("Collection contains no elements");

            return list[System.Random.Shared.Next(list.Count)];
        }

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