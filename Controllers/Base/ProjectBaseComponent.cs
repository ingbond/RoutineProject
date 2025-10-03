using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoutineProject.Security;

namespace RoutineProject.Controllers.Base;

[Authorize]
[Route("projects/{projectId:guid}/[controller]")]
[ProjectAuthorize()]
[ApiController]
public abstract class ProjectBaseController : ControllerBase
{
}
