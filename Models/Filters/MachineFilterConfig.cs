using RoutineProject.Entities;
using System.Text.Json;

namespace RoutineProject.Models.Filters;

public static class MachineFilterConfig
{
  public static void Configure(FilterRegistry<Machine> registry)
  {
    registry.RegisterFilter(
        key: JsonNamingPolicy.CamelCase.ConvertName(nameof(Machine.SerialNumber)),
        valueExtractor: machine => !string.IsNullOrEmpty(machine.SerialNumber)
            ? [machine.SerialNumber]
            : [],
        predicateBuilder: (machine, selectedValues) =>
            selectedValues.Contains(machine.SerialNumber)
    );

    registry.RegisterFilter(
        key: JsonNamingPolicy.CamelCase.ConvertName(nameof(Machine.Location)),
        valueExtractor: machine => !string.IsNullOrEmpty(machine.Location)
            ? [machine.Location]
            : [],
        predicateBuilder: (machine, selectedValues) =>
            selectedValues.Contains(machine.Location)
    );
  }
}