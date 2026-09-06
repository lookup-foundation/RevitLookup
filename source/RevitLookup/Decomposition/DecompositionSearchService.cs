using System.Diagnostics.CodeAnalysis;
using RevitLookup.Abstractions.Decomposition;

namespace RevitLookup.Decomposition;

/// <summary>
///     Provides the default implementation of <see cref="IDecompositionSearchService" />.
/// </summary>
/// <remarks>
///     A query matches an object by name, type name, or description, and matches a member by name, all case-insensitively.
///     The service remembers the object selected on the previous call and keeps searching within it while the query only narrows that object's members.
/// </remarks>
[SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
[SuppressMessage("ReSharper", "ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator")]
public sealed class DecompositionSearchService : IDecompositionSearchService
{
    private ObservableDecomposedObject? _previousSelection;

    /// <inheritdoc />
    public (List<ObservableDecomposedObject>, List<ObservableDecomposedMember>) Search(
        string query,
        ObservableDecomposedObject? selectedObject,
        List<ObservableDecomposedObject> objects)
    {
        try
        {
            if (selectedObject is not null)
            {
                return SearchMembers(query, selectedObject, objects);
            }

            var fetchedObject = FindPreviousSelectedType(objects);
            if (fetchedObject is not null)
            {
                if (fetchedObject.Members.Count > 0)
                {
                    return SearchMembers(query, fetchedObject, objects);
                }
            }

            var filteredObjects = FilterObjects(query, objects);
            return (filteredObjects, []);
        }
        finally
        {
            if (query == string.Empty)
            {
                _previousSelection = null;
            }
            else if (selectedObject is not null)
            {
                _previousSelection = selectedObject;
            }
        }
    }

    /// <inheritdoc />
    public List<ObservableDecomposedMember> SearchMembers(string query, ObservableDecomposedObject value)
    {
        var filteredMembers = FilterMembers(query, value.Members);
        return filteredMembers.Count == 0 ? value.Members : filteredMembers;
    }

    private static (List<ObservableDecomposedObject>, List<ObservableDecomposedMember>) SearchMembers(
        string query,
        ObservableDecomposedObject obj,
        List<ObservableDecomposedObject> objects)
    {
        var filterData = FilterMembers(query, obj.Members);
        if (query.Length > 0 && filterData.Count > 0)
        {
            var typedObjects = FilterTypes(obj, objects);
            return (typedObjects, filterData);
        }

        var filterObjects = FilterObjects(query, objects);
        return (filterObjects, obj.Members);
    }

    private static List<ObservableDecomposedObject> FilterTypes(ObservableDecomposedObject query, IEnumerable<ObservableDecomposedObject> data)
    {
        var filteredObjects = new List<ObservableDecomposedObject>();
        foreach (var item in data)
        {
            if (item.TypeFullName == query.TypeFullName)
            {
                filteredObjects.Add(item);
            }
        }

        return filteredObjects;
    }

    private static List<ObservableDecomposedObject> FilterObjects(string query, List<ObservableDecomposedObject> objects)
    {
        if (query.Length == 0)
        {
            return objects;
        }

        var filteredObjects = new List<ObservableDecomposedObject>();
        foreach (var item in objects)
        {
            if (item.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                item.TypeName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                (item.Description is not null && item.Description.Contains(query, StringComparison.OrdinalIgnoreCase)))
            {
                filteredObjects.Add(item);
            }
        }

        return filteredObjects;
    }

    private static List<ObservableDecomposedMember> FilterMembers(string query, List<ObservableDecomposedMember> members)
    {
        if (query.Length == 0)
        {
            return members;
        }

        var filteredMembers = new List<ObservableDecomposedMember>();
        foreach (var item in members)
        {
            if (item.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            {
                filteredMembers.Add(item);
            }
        }

        return filteredMembers;
    }

    private ObservableDecomposedObject? FindPreviousSelectedType(List<ObservableDecomposedObject> decomposedObjects)
    {
        if (_previousSelection is null)
        {
            return null;
        }

        ObservableDecomposedObject? fetchedObject = null;
        foreach (var decomposedObject in decomposedObjects)
        {
            if (decomposedObject.TypeFullName != _previousSelection.TypeFullName)
            {
                continue;
            }

            if (decomposedObject.Members.Count == 0)
            {
                continue;
            }

            fetchedObject = decomposedObject;
            break;
        }

        return fetchedObject;
    }
}
