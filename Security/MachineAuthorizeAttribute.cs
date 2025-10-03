using Microsoft.AspNetCore.Mvc.Filters;
using RoutineProject.Context;
using RoutineProject.Utils;

namespace RoutineProject.Security;

public class MachineAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
  private readonly string _machineIdRouteKey;

  public MachineAuthorizeAttribute(string machineIdRouteKey = "machineId")
  {
    _machineIdRouteKey = machineIdRouteKey;
  }

  public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
  {
    var dbContext = context.HttpContext.RequestServices.GetRequiredService<MainDbContext>();
    var user = await UserUtil.GetDbUserViaClaimsAsync(dbContext, context.HttpContext.User);

    if (user == null)
    {
      context.Result = new Microsoft.AspNetCore.Mvc.UnauthorizedResult();
      return;
    }

    // Get machineId from route data
    var routeData = context.RouteData.Values[_machineIdRouteKey];
    if (routeData == null || !Guid.TryParse(routeData.ToString(), out Guid machineId))
    {
      context.Result = new Microsoft.AspNetCore.Mvc.BadRequestResult(); // Or Unauthorized if invalid
      return;
    }

    var isUserHasAccessToMachine = await UserUtil.IsUserHasAccessToMachine(dbContext, user.Id, machineId);

    if (!isUserHasAccessToMachine)
    {
      context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
    }
  }
}