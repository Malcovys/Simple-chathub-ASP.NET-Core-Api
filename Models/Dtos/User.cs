using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace Models.Dtos;

public class User
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required bool IsConnected { get; set; } = false;

    [JsonIgnore]
    public string? Password { get; set; }
}

public sealed class UserDb: DbContext
{
    public UserDb(DbContextOptions<UserDb> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
}