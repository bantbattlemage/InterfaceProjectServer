using Newtonsoft.Json;
using System;

namespace GameComms
{
	[JsonObject]
	public abstract class Response
    {
		public bool Success { get; set; }
		public string Message { get; set; }
	}

	[JsonObject]
	public class LogInResponse : Response
	{
		public string AccessKey { get; set; }
		public string Username { get; set; }
		public int UserId { get; set; }
	}

	[JsonObject]
	public class GetChatRoomResponse : Response
	{
		public ChatRoom Room { get; set; }
	}

	[JsonObject]
	public class GetUserResponse : Response
	{
		public GameUser User { get; set; }
	}

	[JsonObject]
	public class ChatJoinResponse : Response
	{
		public ChatUser AssignedChatUser { get; set; }
		public ChatMessage[] ChatMessages { get; set; }
	}

	[JsonObject]
	public class ChatMessageResponse : Response
	{
		public ChatMessage[] ChatMessages { get; set; }
	}

	[JsonObject]
	public class ChatUserResponse : Response
    {
		public ChatUser ChatRoomUser { get; set; }
	}
}