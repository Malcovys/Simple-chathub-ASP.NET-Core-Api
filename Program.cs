using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Models.Dtos;
using SignalRChat.Hubs;

var builder = WebApplication.CreateSlimBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ChatHub") ?? "Data Source=ChatHub.db";

builder.Services.AddSqlite<UserDb>(connectionString);
builder.Services.AddSqlite<MessageDb>(connectionString);
// builder.Services.AddDbContext<UserDb>(options => options.UseInMemoryDatabase("users"));

// builder.Services.AddSignalR();
builder.Services.AddSignalR()
// .AddHubOptions(option => 
//     option.AddFilter<ChatHubFilter>()
// )
.AddJsonProtocol(option =>
    option.PayloadSerializerOptions.TypeInfoResolver = AppJsonSerializerContext.Default
);

builder.Services.ConfigureHttpJsonOptions(option =>
    option.SerializerOptions.TypeInfoResolverChain.Insert(0, AppJsonSerializerContext.Default)
);

var app = builder.Build();

app.MapGroup("/auth")
    .MapAuthApi();

app.MapGroup("/users")
    .MapUserApi();

app.MapGroup("/messages")
    .MapChatApi();

app.MapHub<ChatHub>("/ChatHub");

app.Run();