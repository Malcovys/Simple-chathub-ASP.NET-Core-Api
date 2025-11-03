namespace Models.Requests;

public record SendMessageRequest(int UserId, string Content);