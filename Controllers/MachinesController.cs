using Microsoft.AspNetCore.Mvc;
using RoutineProject.Controllers.Base;
using RoutineProject.Models;
using RoutineProject.Models.Dtos;
using RoutineProject.Models.Pagination;
using RoutineProject.Services;

namespace RoutineProject.Controllers;

public class MachinesController(MachinesService machinesService) : ProjectBaseController
{
    [HttpGet]
    public async Task<ActionResult<PaginationResult<MachineDto>>> GetPaginated([FromRoute] Guid projectId,
        [FromQuery] GetMachinesListPaginationMeta paginationMeta)
    {
        try
        {
            var result = await machinesService.GetPaginatedAsync(User, projectId, paginationMeta);
            return result;
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpGet("metadata")]
    public async Task<ActionResult<MachineListMeta>> GetMetadata([FromRoute] Guid projectId)
    {
        try
        {
            var result = await machinesService.GetMetadataAsync(projectId);
            return result;
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<MachineDto>> Create([FromBody] MachineCreateDto machineDto, [FromRoute] Guid projectId)
    {
        try
        {
            var result = await machinesService.CreateMachineAsync(User, machineDto, projectId);
            return result;
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpPut]
    public async Task<ActionResult<MachineDto>> Update([FromBody] MachineUpdateDto machineDto, [FromRoute] Guid projectId)
    {
        try
        {
            var result = await machinesService.UpdateMachineAsync(User, machineDto, projectId);
            return result;
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete]
    [Route("{machineId:guid}")]
    public async Task<ActionResult> Delete([FromRoute] Guid projectId, [FromRoute] Guid machineId)
    {
        try
        {
            await machinesService.DeleteMachineAsync(User, projectId, machineId);
            return Ok();
        }
        catch (UnauthorizedAccessException e)
        {
            return Forbid(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}