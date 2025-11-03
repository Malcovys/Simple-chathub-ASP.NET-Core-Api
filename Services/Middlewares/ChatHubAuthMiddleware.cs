using Models.Dtos;

namespace Middleware;

public class ChatHubAuthMiddleware
{
    private readonly RequestDelegate _next;

    public ChatHubAuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, UserDb db)
    {
        if (context.Request.Path.StartsWithSegments("/ChatHub"))
        {
            var userIdString = context.Request.Query["userId"].ToString();

            if (string.IsNullOrEmpty(userIdString))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("userId is required");
                return;
            }

            if (!int.TryParse(userIdString, out int userId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("userId must be a valid integer");
                return;
            }

            // Retrive user from database
            // var db = context.RequestServices.GetRequiredService<UserDb>();
            var user = await db.Users.FindAsync(userId);

            if (user is null)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("unknown user");
                return;
            }

            // Add user data to the context
            context.Items["UserId"] = user.Id;
        }

        await _next(context);
    }
}

public static class ChatHubAuthMiddlewareExtentions
{
    public static IApplicationBuilder UseChatHubAuth(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ChatHubAuthMiddleware>();
    }
}