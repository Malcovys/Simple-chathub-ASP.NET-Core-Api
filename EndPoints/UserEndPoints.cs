using Microsoft.EntityFrameworkCore;
using Models.Dtos;

public static class UserEndPoints
{
    internal static RouteGroupBuilder MapUserApi(this RouteGroupBuilder group)
    {
        group.MapGet("/connected", async (UserDb db) =>
        {
            var connected = await db.Users.Where(usr =>
                usr.IsConnected
            ).ToListAsync();
            
            return Results.Ok(connected);
        });

        return group;
    }
}