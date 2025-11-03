using Microsoft.EntityFrameworkCore;

namespace Models.Dtos;

public class Message
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Content { get; set; }
}

public sealed class MessageDb: DbContext
{
    public MessageDb(DbContextOptions<MessageDb> options) : base(options) { }
    public DbSet<Message> Messages { get; set; } = null!;
}



