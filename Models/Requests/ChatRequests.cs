namespace Models.Requests;

public record UserMessage(int UserId, string UserName, string Content);

public record SendMessageRequest(int UserId, string Content);
public record GetMessagesRequest(List<UserMessage> Messages);
