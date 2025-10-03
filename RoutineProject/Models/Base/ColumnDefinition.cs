using System.Text.Json;

namespace RoutineProject.Models.Base;

public abstract class BaseColumnFilter
{
    public BaseColumnFilter(string filterName)
    {
        FilterName = JsonNamingPolicy.CamelCase.ConvertName(filterName);
    }

    public string FilterName { get; }
}

public class ColumnFilterData : BaseColumnFilter
{
    public ColumnFilterData(string name, List<string>? availableValues) : base(name)
    {
        AvailableValues = availableValues ?? new List<string>();
    }

    public List<string> AvailableValues { get; }
}

public class ColumnToggleData : BaseColumnFilter
{
    public ColumnToggleData(string name) : base(name)
    {
    }
}

public class ColumnDefinition
{
    public ColumnDefinition(
        string name,
        bool isSortable = false,
        bool isEditable = false,
        ColumnFilterData? filter = null,
        ColumnToggleData? toggle = null,
        int? basisRem = null
    )
    {
        Name = JsonNamingPolicy.CamelCase.ConvertName(name);
        IsSortable = isSortable;
        IsEditable = isEditable;
        Filter = filter;
        Toggle = toggle;
        BasisRem = basisRem;
    }


    public int? BasisRem { get; set; }
    public string Name { get; }
    public bool IsSortable { get; set; }
    public bool IsEditable { get; }
    public ColumnFilterData? Filter { get; }
    public ColumnToggleData? Toggle { get; }
}