using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using Models.Requests;
using SignalRChat.Hubs;

public static class ChatEndPoints
{
    internal static RouteGroupBuilder MapChatApi(this RouteGroupBuilder group)
    {
        group.MapGet("/", async (MessageDb Msgdb, UserDb usrDb) => {
            var userMessages = new List<UserMessage>();

            var messages = await Msgdb.Messages.ToListAsync();
            var userIds = messages.Select(m => m.UserId).Distinct().ToList();
            
            var users = await usrDb.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            foreach (var msg in messages)
            {
                if (users.TryGetValue(msg.UserId, out var userName))
                {
                    userMessages.Add(new UserMessage(msg.UserId, userName, msg.Content));
                }
            }

            return userMessages;
        });
        
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

            await hub.Clients.All.SendAsync("ReceiveMessage", request.UserId, user.Name, request.Content);

            return Results.Ok();
        });

        return group;
    }
}