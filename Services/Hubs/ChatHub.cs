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
        var userId = Context.GetHttpContext()?.Items["UserId"];

        if (userId is int id)
        {
            // Mark user is connected
            var userTracked = await _db.Users.FindAsync(id);
            if (userTracked != null)
            {
                userTracked.IsConnected = true;
                await _db.SaveChangesAsync();

                _usersConnections[id] = Context.ConnectionId;

                await Clients.All.SendAsync("UserConnected", id, userTracked.Name);
            }

            Console.WriteLine($"#ChatHub : User {id} connected successfully");
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

                await Clients.All.SendAsync("UserDisconnected", userId, user.Name);
            }

            Console.WriteLine($"#ChatHub : User {userId} disconnected successfully");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
