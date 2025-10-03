using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineProject.Security;

namespace RoutineProject.Controllers.Base;

[Authorize]
[Route("machines/{machineId:guid}/[controller]")]
[MachineAuthorize()]
[ApiController]
public class MachineBaseComponent : ControllerBase
{
}
