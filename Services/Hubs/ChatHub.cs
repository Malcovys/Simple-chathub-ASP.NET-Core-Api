using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR;
using Models.Dtos;

namespace SignalRChat.Hubs;

public class ChatHub : Hub
{
    private UserDb _db;
    private static readonly ConcurrentDictionary<int, string> _usersConnections = new();

    public ChatHub(UserDb db)
    {
        _db = db;
    }

    public override async Task OnConnectedAsync()
    {
        var user = Context.GetHttpContext()?.Items["User"] as User;

        if (user != null)
        {
            // Mark user is connected
            var userTracked = await _db.Users.FindAsync(user.Id);
            if (userTracked != null)
            {
                userTracked.IsConnected = true;
                await _db.SaveChangesAsync();
            }

            _usersConnections[user.Id] = Context.ConnectionId;

            await Clients.Others.SendAsync("UserConnected", user.Id);

            Console.WriteLine($"#ChatHub : User {user.Id} connected successfully");
        }
        
        await base.OnConnectedAsync();
    }

    public override  async Task OnDisconnectedAsync(Exception? exception)
    {
        // Retrive connection
        var userEntity = _usersConnections.FirstOrDefault(
            (c) => c.Value == Context.ConnectionId
        );

        if (userEntity.Key != 0)
        {
            var userId = userEntity.Key;

            // Remove user connection
            _usersConnections.TryRemove(userId, out _);

            // Load user from DB
            var user = await _db.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsConnected = false;
                await _db.SaveChangesAsync();
            }

            await Clients.Others.SendAsync("UserDisconnected", userId);

            Console.WriteLine($"#ChatHub : User {userId} disconnected successfully");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
