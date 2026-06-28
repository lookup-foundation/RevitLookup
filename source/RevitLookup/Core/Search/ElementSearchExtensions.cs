namespace RevitLookup.Core.Search;

[PublicAPI]
public static class ElementSearchExtensions
{
    [Pure]
    public static List<Element> SearchElements(this Document document, string searchText)
    {
        ArgumentNullException.ThrowIfNull(document);

        var rows = searchText.Split([Environment.NewLine], StringSplitOptions.RemoveEmptyEntries);
        var items = ParseRawRequest(rows);
        var results = new List<Element>(items.Count);

        foreach (var rawId in items)
        {
#if REVIT2024_OR_GREATER
            if (long.TryParse(rawId, out var id))
            {
                var element = document.GetElement(new ElementId(id));
#else
            if (int.TryParse(rawId, out var id))
            {
                var element = document.GetElement(new ElementId(id));
#endif
                if (element is not null) results.Add(element);
            }
            else if (rawId.Length == 45 && rawId.Count(static c => c == '-') == 5)
            {
                var element = document.GetElement(rawId);
                if (element is not null) results.Add(element);
            }
            else if (rawId.Length == 22 && rawId.All(static c => c != ' '))
            {
                var elements = SearchByIfcGuid(document, rawId);
                results.AddRange(elements);
            }
            else
            {
                var elements = SearchByName(document, rawId);
                results.AddRange(elements);
            }
        }

        return results;
    }

    private static List<string> ParseRawRequest(string[] rows)
    {
        var items = new List<string>(rows.Length);
        var delimiters = new[] {'\t', ';', ',', ' '};
        foreach (var row in rows)
        {
            for (var i = 0; i < delimiters.Length; i++)
            {
                var delimiter = delimiters[i];
                var split = row.Split([delimiter], StringSplitOptions.RemoveEmptyEntries);
                if (split.Length > 1 || i == delimiters.Length - 1 || split.Length == 1 && split[0] != row)
                {
                    items.AddRange(split);
                    break;
                }
            }
        }

        return items;
    }

    private static IEnumerable<Element> SearchByName(Document document, string rawId)
    {
        return document.CollectElements().Types()
            .UnionWith(document.CollectElements().Instances())
            .Where(element => element.Name.Contains(rawId, StringComparison.OrdinalIgnoreCase));
    }

    private static IList<Element> SearchByIfcGuid(Document document, string rawId)
    {
        var typeGuidsCollector = document.CollectElements()
            .WhereParameter(BuiltInParameter.IFC_TYPE_GUID).Equals(rawId);

        return document.CollectElements()
            .WhereParameter(BuiltInParameter.IFC_GUID).Equals(rawId)
            .UnionWith(typeGuidsCollector)
            .ToElements();
    }
}