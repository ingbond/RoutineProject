using RoutineProject.AutoMapper;
using RoutineProject.Entities;

namespace RoutineProject.Models.Dtos;

public class MachineUpdateDto : IMapFrom<Machine>
{
    public Guid Id { get; set; }
    public required string SerialNumber { get; set; }
    public string? Location { get; set; }
}
