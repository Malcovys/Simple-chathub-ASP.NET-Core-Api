# ChatHub Server

Real-time chat application built with ASP.NET Core and SignalR.

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQLite (included with .NET)

## Installation

1. **Clone the project**
   ```bash
   git clone <repository-url>
   cd ChatHub/Server
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update --context UserDb
   dotnet ef database update --context MessageDb
   ```

   If `dotnet ef` is not installed:
   ```bash
   dotnet tool install --global dotnet-ef
   ```

## Running the Server

```bash
dotnet run
```

The server starts on `http://localhost:3000` by default.

## Available Endpoints

### Authentication
- `POST /auth/register` - Create a new user
- `POST /auth/login` - Login

### Users
- `GET /users` - List all users
- `GET /users/connected` - List connected users

### Messages
- `GET /messages` - Retrieve all messages
- `POST /messages` - Send a new message

### WebSocket (SignalR)
- `/ChatHub?userId={id}` - WebSocket connection for real-time chat

## Project Structure

```
Server/
├── EndPoints/          # REST API endpoints
├── Services/
│   ├── Hubs/          # SignalR ChatHub
│   └── Middlewares/   # Authentication middleware
├── Models/
│   ├── Dtos/          # DbContext and entities
│   └── Requests/      # Request models
├── Migrations/        # Entity Framework migrations
└── Program.cs         # Entry point
```

## Configuration

Configuration is located in `appsettings.json` and `appsettings.Development.json`.

### Connection String
SQLite is used by default:
```json
{
  "ConnectionStrings": {
    "ChatHub": "Data Source=ChatHub.db"
  }
}
```

### Log Levels
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

## Development

### Create a new migration

For UserDb:
```bash
dotnet ef migrations add <MigrationName> --context UserDb
```

For MessageDb:
```bash
dotnet ef migrations add <MigrationName> --context MessageDb
```

### Clean and rebuild
```bash
dotnet clean
dotnet build
```

## Client Integration

The client connects via:
1. **HTTP** for REST requests (`/auth`, `/users`, `/messages`)
2. **WebSocket** for SignalR (`/ChatHub?userId={id}`)

### SignalR Events Emitted by Server

- `ReceiveMessage(int userId, string userName, string content)` - New message received
- `UserConnected(int id, string name)` - User connected
- `UserDisconnected(int id, string name)` - User disconnected

## Troubleshooting

### Issue: "Cannot resolve scoped service from root provider"
**Solution**: Scoped services (DbContext) cannot be injected into middleware constructors. Use `context.RequestServices.GetRequiredService<T>()` inside `InvokeAsync`.

### Issue: Logs are too verbose
**Solution**: Adjust log levels in `appsettings.Development.json` (see Configuration section).

### Issue: Migrations don't apply
**Solution**: 
```bash
dotnet ef database drop --context UserDb
dotnet ef database drop --context MessageDb
dotnet ef database update --context UserDb
dotnet ef database update --context MessageDb
```

## 👤 Author

**Malcovys**
- GitHub: [@Malcovys](https://github.com/Malcovys)
- Repository: [Simple-chathub-Client-Avalonia](https://github.com/Malcovys/Simple-chathub-Client-Avalonia)