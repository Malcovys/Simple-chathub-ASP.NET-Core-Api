using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Models.Requests;

public static class AuthEndPoints
{
    internal static RouteGroupBuilder MapAuthApi(this RouteGroupBuilder group)
    {
        group.MapPost("/", async (UserDb db, LoginRequest request) =>
        {
            var user = await db.Users.FirstOrDefaultAsync((usr) =>
                usr.Name == request.Name && usr.Password == request.Password
            );

            if (user is null) return Results.Unauthorized();

            // user.IsConnected = true;
            // await db.SaveChangesAsync();

            return Results.Ok(new { userId = user.Id });
        });

        group.MapPost("/register", async (UserDb db, RegisterRequest request) =>
        {
            var userNameAlreadyExist = await db.Users.FirstOrDefaultAsync((usr) =>
                usr.Name == request.Name
            );

            if (userNameAlreadyExist != null)
                return Results.Conflict("User name already in use");

            var newUser = new User
            {
                Name = request.Name,
                IsConnected = false,
                Password = request.Password
            };

            await db.Users.AddAsync(newUser);
            await db.SaveChangesAsync();

            return Results.Created();
        });

        return group;
    }
}