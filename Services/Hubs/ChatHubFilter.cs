using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Models.Dtos;

public class ChatHubFilter : IHubFilter
{
    private UserDb _db;
    private static readonly ConcurrentDictionary<int, string> _usersConnections = new();

    public ChatHubFilter(UserDb db)
    {
        _db = db;
    }

    public async ValueTask<object?> InvokeMethodAsync(
        HubInvocationContext invocationContext,
        Func<HubInvocationContext, ValueTask<object?>> next
    )
    {
        Console.WriteLine($"Calling hub method '{invocationContext.HubMethodName}'");
        
        try
        {
            return await next(invocationContext);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            throw;
        }
    }

    public async Task OnConnectedAsync(
        HubLifetimeContext context,
        Func<HubLifetimeContext, Task> next
    )
    {
        // Validate request
        var httpContext = context.Context.GetHttpContext();
        var userIdString = httpContext?.Request.Query["userId"].ToString();

        if (string.IsNullOrEmpty(userIdString))
        {
            context.Hub.Context.Abort();
            throw new HubException("userId is required");
        }

        if (!int.TryParse(userIdString, out int userId))
        {
            context.Hub.Context.Abort();
            throw new HubException("userId must be a valid integer");
        }

        // Retrive user from database
        var user = await _db.Users.FindAsync(userId);

        if (user is null)
        {
            throw new HubException("user unknow");
        }
        
        Console.WriteLine($"User attempting to connect: {userIdString}");

        await next(context);
        
        // Mark user is connected
        user.IsConnected = true;
        await _db.SaveChangesAsync();

        _usersConnections[userId] = context.Context.ConnectionId;
        
        Console.WriteLine($"User {userId} connected successfully");
    }

    public async Task OnDisconnectedAsync(
        HubLifetimeContext context,
        Exception? exception,
        Func<HubLifetimeContext, Exception?, Task> next)
    {
        Console.WriteLine("User disconnecting...");

        // Retrive connection
        var userEntity = _usersConnections.FirstOrDefault((connections) =>
            connections.Value == context.Context.ConnectionId
        );

        if (userEntity.Key != 0)
        {
            // Remive user connection
            var userId = userEntity.Key;
            var removed = _usersConnections.TryRemove(userId, out _);

            // Notify user disconnected
            if(removed)
            {   
                var user = await _db.Users.FindAsync(userId);
                if (user != null)
                {
                    user.IsConnected = false;
                    await _db.SaveChangesAsync();
                }
            }
        }

        await next(context, exception);
        
        // APRÈS OnDisconnectedAsync du Hub
        Console.WriteLine("User disconnected");
    }
}