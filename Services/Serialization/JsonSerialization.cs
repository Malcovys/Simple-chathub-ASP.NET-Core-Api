using System.Text.Json.Serialization;
using Models.Dtos;
using Models.Requests;


[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(User))]
[JsonSerializable(typeof(List<User>))]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(List<Message>))]
[JsonSerializable(typeof(LoginRequest))]
[JsonSerializable(typeof(RegisterRequest))]
[JsonSerializable(typeof(GetConnectedUsersRequest))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{

}