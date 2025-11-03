namespace Models.Requests;

public record LoginRequest(string Name, string Password);
public record RegisterRequest(string Name, string Password);
public record LogoutRequest(int userId);
