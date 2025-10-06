using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using RoutineProject.Context;
using RoutineProject.Entities;
using RoutineProject.Extensions;
using RoutineProject.Models;
using RoutineProject.Models.Base;
using RoutineProject.Models.Dtos;
using RoutineProject.Models.Filters;
using RoutineProject.Models.Pagination;
using RoutineProject.Utils;
using System.Linq.Expressions;
using System.Security.Claims;

namespace RoutineProject.Services;

public class MachinesService
{
    private readonly MainDbContext _mainDbContext;
    private readonly IMapper _mapper;
    private readonly FilterRegistry<Machine> _machineFilterRegistry;
    private readonly Dictionary<string, Expression<Func<Machine, object>>> _sortHeaderMapping =
        new(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(MachineDto.SerialNumber)] = x => x.SerialNumber,
        };

    public MachinesService(MainDbContext mainDbContext,
      IMapper mapper)
    {
        _mainDbContext = mainDbContext;
        _mapper = mapper;
        _machineFilterRegistry = new FilterRegistry<Machine>();
        MachineFilterConfig.Configure(_machineFilterRegistry);
    }

    public async Task<PaginationResult<MachineDto>> GetPaginatedAsync(ClaimsPrincipal user,
      Guid projectId,
      GetMachinesListPaginationMeta paginationMeta)
    {
        var dbUser = await UserUtil.GetDbUserViaClaimsAsync(_mainDbContext, user);
        var query = _mainDbContext.UserProjectMappings
          .Where(x => x.UserId == dbUser.Id && x.ProjectId == projectId)
          .SelectMany(x => x.Project.Machines)
          .AsNoTracking()
          .MakeSorting(paginationMeta, _sortHeaderMapping);

        if (paginationMeta.From is not null)
        {
            query = query.Where(x => x.CreatedAt >= paginationMeta.From);
        }

        if (paginationMeta.To is not null)
        {
            query = query.Where(x => x.CreatedAt <= paginationMeta.To);
        }

        if (paginationMeta.SearchTerm is not null)
        {
            query = query.Where(x =>
              x.Location != null && x.Location.Contains(paginationMeta.SearchTerm)
            );
        }

        query = query.TryFilterQueryByFilterGroups(_machineFilterRegistry, paginationMeta);

        var totalCount = await query.CountAsync();
        var result = await query
          .Paginate(paginationMeta)
          .ProjectTo<MachineDto>(_mapper.ConfigurationProvider)
          .ToListAsync();

        var paginator = new PaginationResult<MachineDto>(result, totalCount, paginationMeta);

        return paginator;
    }

    public async Task<MachineDto> CreateMachineAsync(ClaimsPrincipal userClaims, MachineCreateDto machineDto, Guid projectId)
    {
        var user = await UserUtil.GetDbUserViaClaimsAsync(_mainDbContext, userClaims);
        var project = await _mainDbContext.UserProjectMappings
          .Where(x => x.UserId == user.Id)
          .Select(x => x.Project)
          .SingleOrDefaultAsync(x => x.Id == projectId) ??
          throw new UnauthorizedAccessException("User not allowed to create such machine");
        var newMachine = _mapper.Map<Machine>(machineDto);
        var createdMachineEntry = await _mainDbContext.Machines.AddAsync(newMachine);

        await _mainDbContext.SaveChangesAsync();

        return _mapper.Map<MachineDto>(createdMachineEntry.Entity);
    }


    public async Task<MachineDto> UpdateMachineAsync(ClaimsPrincipal userClaims, MachineUpdateDto machineDto, Guid projectId)
    {
        var user = await UserUtil.GetDbUserViaClaimsAsync(_mainDbContext, userClaims);
        var machine = await GetMachineOrThrow(machineDto.Id, projectId, user);
        _mapper.Map(machineDto, machine);

        await _mainDbContext.SaveChangesAsync();

        return _mapper.Map<MachineDto>(machine);
    }

    public async Task DeleteMachineAsync(ClaimsPrincipal userClaims, Guid projectId, Guid machineId)
    {
        var user = await UserUtil.GetDbUserViaClaimsAsync(_mainDbContext, userClaims);
        var machine = await GetMachineOrThrow(machineId, projectId, user);
        _mainDbContext.Remove(machine);

        await _mainDbContext.SaveChangesAsync();
    }

    public async Task<MachineListMeta> GetMetadataAsync(Guid projectId)
    {
        var dbMachines = await _mainDbContext.Machines
         .AsNoTracking()
         .Where(x => x.ProjectId == projectId)
         .ToListAsync();
        var columns = new List<ColumnDefinition>
        {
            new(nameof(MachineDto.SerialNumber), basisRem: 22),
            new(nameof(MachineDto.Location)),
        };

        foreach (var item in columns)
        {
            if (_sortHeaderMapping.ContainsKey(item.Name))
            {
                item.IsSortable = true;
            }
        }

        return new MachineListMeta
        {
            Columns = columns,
            FilterGroups = _machineFilterRegistry.BuildFilterGroups(dbMachines)
        };
    }

    private async Task<Machine> GetMachineOrThrow(Guid machineId, Guid projectId, User user)
    {
        return await _mainDbContext.UserProjectMappings
          .Include(x => x.Project.Machines)
          .Where(x => x.UserId == user.Id && x.ProjectId == projectId)
          .SelectMany(x => x.Project.Machines)
          .SingleOrDefaultAsync(x => x.Id == machineId) ?? throw new UnauthorizedAccessException("No machine found or no access to machine");
    }
}