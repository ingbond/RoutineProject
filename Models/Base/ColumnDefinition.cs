using System.Text.Json;

namespace RoutineProject.Models.Base;

public abstract class BaseColumnFilter(string filterName)
{
    public string FilterName { get; } = JsonNamingPolicy.CamelCase.ConvertName(filterName);
}

public class ColumnFilterData(string name, List<string>? availableValues) : BaseColumnFilter(name)
{
    public List<string> AvailableValues { get; } = availableValues ?? new List<string>();
}

public class ColumnToggleData(string name) : BaseColumnFilter(name)
{
}

public class ColumnDefinition(
    string name,
    bool isSortable = false,
    bool isEditable = false,
    ColumnFilterData? filter = null,
    ColumnToggleData? toggle = null,
    int? basisRem = null
    )
{
    public int? BasisRem { get; set; } = basisRem;
    public string Name { get; } = JsonNamingPolicy.CamelCase.ConvertName(name);
    public bool IsSortable { get; set; } = isSortable;
    public bool IsEditable { get; } = isEditable;
    public ColumnFilterData? Filter { get; } = filter;
    public ColumnToggleData? Toggle { get; } = toggle;
}