using Microsoft.EntityFrameworkCore;
using RoutineProject.Context;
using RoutineProject.Entities;
using System.Security.Claims;

namespace RoutineProject.Utils;

public static class UserUtil
{
    public static async Task<User> GetDbUserViaClaimsAsync(MainDbContext context, ClaimsPrincipal claimsUser)
    {
        var userId = Guid.Parse(claimsUser.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return await context.Users.SingleAsync(x => x.Id == userId);
    }

    public static async Task<bool> IsUserHasAccessToMachine(MainDbContext context, Guid userId, Guid machineId)
    {
        return await context.UserProjectMappings
            .AnyAsync(x => x.UserId == userId && x.Project.Machines.Any(m => m.Id == machineId));
    }

    public static async Task<bool> IsUserHasAccessToProject(MainDbContext context, Guid userId, Guid projectId)
    {
        return await context.UserProjectMappings
            .AnyAsync(x => x.UserId == userId && x.Project.Id == projectId);
    }
}