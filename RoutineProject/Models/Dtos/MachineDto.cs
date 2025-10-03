using AutoMapper;
using RoutineProject.AutoMapper;
using RoutineProject.Entities;

namespace RoutineProject.Models.Dtos;

public class MachineDto : IMapFrom<Machine>
{
    public required string SerialNumber { get; set; }
    public string? Location { get; set; }
    public string? LocationWithSN { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Machine, MachineDto>()
          .ForMember(x => x.LocationWithSN, d => d.MapFrom(z => $"SN: {z.SerialNumber} L: {z.Location}"));
    }
}
