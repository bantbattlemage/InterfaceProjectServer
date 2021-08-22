using Newtonsoft.Json;
using System;

namespace GameComms
{
	[JsonObject]
	public abstract class GameData
	{
		public int Id { get; set; }
    }

	[JsonObject]
	public class GameUser : GameData
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public DateTime RegistrationDate { get; set; }
	}

	[JsonObject]
	public class LogInSession : GameData
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public string AccessToken { get; set; }
		public DateTime LastAccess { get; set; }
	}

	[JsonObject]
	public class ChatRoom : GameData
	{
		public string ChatRoomName { get; set; }
		public bool PrivateChat { get; set; }
		public int PrivateChatOwner { get; set; }
	}

	[JsonObject]
	public class ChatMessage : GameData
	{
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public string Username { get; set; }
		public string Message { get; set; }
		public DateTime TimeStamp { get; set; }
	}

	[JsonObject]
	public class ChatUser : GameData
	{
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public string Username { get; set; }
		public DateTime ActiveTime { get; set; }
	}
}
