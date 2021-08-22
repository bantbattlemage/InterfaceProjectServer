using Newtonsoft.Json;
using System;

namespace GameComms
{
	[JsonObject]
	public abstract class Request
    {
    }

	[JsonObject]
	public class LogInRequest : Request
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public bool NewRegistration { get; set; }
	}

	[JsonObject]
	public class GetUserRequest : Request
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	[JsonObject]
	public class JoinChatRoomRequest : Request
    {
		public int SenderUserId { get; set; }
		public int ChatRoomId { get; set; }
		public string Username { get; set; }
	}

	[JsonObject]
	public class ChatMessagePostRequest : Request
	{
		public int SenderUserId { get; set; }
		public int ChatRoomId { get; set; }
		public string Username { get; set; }
		public string Message { get; set; }
		public string TimeStamp { get; set; }
	}

	[JsonObject]
	public class ChatMessageReadRequest : Request
	{
		public int SenderUserId { get; set; }
		public int ChatRoomId { get; set; }
		public DateTime LastMessageRead { get; set; }
	}

	[JsonObject]
	public class ChatMessageReadRequests : Request
	{
		public ChatMessageReadRequest[] Requests;
	}
}
