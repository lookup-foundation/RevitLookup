using System.Reflection;
using System.Text;
using RevitLookup.Abstractions.Tools;

namespace RevitLookup.Tools.Units;

/// <summary>
///     Provides methods that collect Revit's built-in parameters, categories, and forge unit type identifiers for display.
/// </summary>
public static class UnitsCollector
{
    /// <summary>
    ///     Collects the built-in parameters defined by <see cref="BuiltInParameter" />.
    /// </summary>
    /// <returns>The collected parameters. A parameter without a label has an empty <see cref="UnitInfo.Label" />.</returns>
    public static List<UnitInfo> GetBuiltinParametersInfo()
    {
        var parameters = Enum.GetValues<BuiltInParameter>();
        var parameterNames = Enum.GetNames<BuiltInParameter>();

        var result = new List<UnitInfo>(parameters.Length);
        for (var i = 0; i < parameters.Length; i++)
        {
            var parameter = parameters[i];
            string label;
            try
            {
                label = parameter.ToLabel();
            }
            catch
            {
                // Some parameters don't have a label
                label = string.Empty;
            }

            result.Add(new UnitInfo
            {
                Unit = parameterNames[i],
                Label = label,
                Value = parameter
            });
        }

        return result;
    }

    /// <summary>
    ///     Collects the built-in categories defined by <see cref="BuiltInCategory" />.
    /// </summary>
    /// <returns>The collected categories. A category without a label has an empty <see cref="UnitInfo.Label" />.</returns>
    public static List<UnitInfo> GetBuiltinCategoriesInfo()
    {
        var categories = Enum.GetValues<BuiltInCategory>();
        var categoryNames = Enum.GetNames<BuiltInCategory>();

        var result = new List<UnitInfo>(categories.Length);
        for (var i = 0; i < categories.Length; i++)
        {
            var category = categories[i];
            string label;
            try
            {
                label = category.ToLabel();
            }
            catch
            {
                // Some categories don't have a label
                label = string.Empty;
            }

            result.Add(new UnitInfo
            {
                Unit = categoryNames[i],
                Label = label,
                Value = category
            });
        }

        return result;
    }

    /// <summary>
    ///     Collects the forge type identifiers defined by the Revit unit, spec, parameter, group, discipline, and symbol type id classes.
    /// </summary>
    /// <returns>The collected identifiers, with their label and declaring class name.</returns>
    public static List<UnitInfo> GetForgeInfo()
    {
        const BindingFlags searchFlags = BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly;

        var dataTypes = new List<PropertyInfo>();
#if REVIT2022_OR_GREATER
        dataTypes.AddRange(typeof(UnitTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SpecTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SpecTypeId.Boolean).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SpecTypeId.String).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SpecTypeId.Int).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SpecTypeId.Reference).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(ParameterTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(GroupTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(DisciplineTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SymbolTypeId).GetProperties(searchFlags));
#else
        dataTypes.AddRange(typeof(UnitTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SpecTypeId).GetProperties(searchFlags));
        dataTypes.AddRange(typeof(SymbolTypeId).GetProperties(searchFlags));
#endif
        return dataTypes.Select(static info =>
            {
                var typeId = (ForgeTypeId)info.GetValue(null)!;
                return new UnitInfo
                {
                    Unit = typeId.TypeId,
                    Label = GetLabel(typeId, info),
                    Class = GetClassName(info),
                    Value = typeId
                };
            })
            .ToList();
    }


    private static string GetClassName(PropertyInfo property)
    {
        var type = property.DeclaringType!;
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(type.Name);
        stringBuilder.Append('.');
        stringBuilder.Append(property.Name);

        while (type.IsNested)
        {
            type = type.DeclaringType!;
            stringBuilder.Insert(0, '.');
            stringBuilder.Insert(0, type.Name);
        }

        return stringBuilder.ToString();
    }

    private static string GetLabel(ForgeTypeId typeId, PropertyInfo property)
    {
        if (typeId.Empty())
        {
            return string.Empty;
        }

        if (property.Name == nameof(SpecTypeId.Custom))
        {
            return string.Empty;
        }

        var type = property.DeclaringType;
        while (type!.IsNested)
        {
            type = type.DeclaringType;
        }

        try
        {
            return type.Name switch
            {
                nameof(UnitTypeId) => typeId.ToUnitLabel(),
                nameof(SpecTypeId) => typeId.ToSpecLabel(),
                nameof(SymbolTypeId) => typeId.ToSymbolLabel(),
#if REVIT2022_OR_GREATER
                nameof(ParameterTypeId) => typeId.ToParameterLabel(),
                nameof(GroupTypeId) => typeId.ToGroupLabel(),
                nameof(DisciplineTypeId) => typeId.ToDisciplineLabel(),
#endif
                _ => throw new ArgumentOutOfRangeException(nameof(typeId), typeId, "Unknown Forge Type Identifier")
            };
        }
        catch
        {
            //Some parameter label thrown an exception
            return string.Empty;
        }
    }
}
