using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Models.Requests;
using SignalRChat.Hubs;

public static class ChatEndPoints
{
    internal static RouteGroupBuilder MapChatApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (MessageDb db) => await db.Messages.ToListAsync());
        
        group.MapPost("/", async (
            UserDb usrDb,
            MessageDb msgdb,
            IHubContext<ChatHub> hub,
            SendMessageRequest request
        ) =>
        {
            var user = await usrDb.Users.FindAsync(request.UserId);

            if (user is null) return Results.Unauthorized();

            var newMessage = new Message
            {
                UserId = user.Id,
                Content = request.Content
            };

            await msgdb.Messages.AddAsync(newMessage);
            await msgdb.SaveChangesAsync();

            await hub.Clients.All.SendAsync("ReceiveMessage", request.UserId, request.Content);

            return Results.Ok();
        });

        return group;
    }
}