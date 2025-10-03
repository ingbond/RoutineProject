using RoutineProject.AutoMapper;
using RoutineProject.Entities;

namespace RoutineProject.Models.Dtos;

public class MachineCreateDto : IMapFrom<Machine>
{
    public required string SerialNumber { get; set; }
    public string? Location { get; set; }
}
