using Microsoft.AspNetCore.Mvc.Filters;
using RoutineProject.Context;
using RoutineProject.Utils;

namespace RoutineProject.Security;

public class ProjectAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
  private readonly string _projectIdRouteKey;

  public ProjectAuthorizeAttribute(string projectIdRouteKey = "projectId")
  {
    _projectIdRouteKey = projectIdRouteKey;
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

    // Get projectId from route data
    var routeData = context.RouteData.Values[_projectIdRouteKey];

    if (routeData == null || !Guid.TryParse(routeData.ToString(), out Guid projectId))
    {
      context.Result = new Microsoft.AspNetCore.Mvc.BadRequestResult(); // Or Unauthorized if invalid
      return;
    }

    var isUserHasAccessToMachine = await UserUtil.IsUserHasAccessToProject(dbContext, user.Id, projectId);

    if (!isUserHasAccessToMachine)
    {
      context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
    }
  }
}