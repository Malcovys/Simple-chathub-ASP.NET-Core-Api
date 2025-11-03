using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs;

public class ChatHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();

        var userIdString = Context.GetHttpContext()?.Request.Query["userId"].ToString();
        
        int userId = int.Parse(userIdString!);
        
        await Clients.Others.SendAsync("UserConnected", userId);
    }

    public override  async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);

        var userIdString = Context.GetHttpContext()?.Request.Query["userId"].ToString();

        int userId = int.Parse(userIdString!);
        
        await Clients.Others.SendAsync("UserDisconnected", userId);
    }
}
