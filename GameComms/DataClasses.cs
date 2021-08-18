using System;

namespace GameComms
{
	public class GameData
    {
		public int Id { get; set; }
    }

	public class GameUser : GameData
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public DateTime RegistrationDate { get; set; }
	}

	public class ChatRoom : GameData
	{
		public string ChatRoomName { get; set; }
		public bool PrivateChat { get; set; }
		public int PrivateChatOwner { get; set; }
	}

	public class ChatMessage : GameData
	{
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public string Username { get; set; }
		public string Message { get; set; }
		public DateTime TimeStamp { get; set; }
	}

	public class ChatUser : GameData
	{
		public int UserId { get; set; }
		public int RoomId { get; set; }
		public string Username { get; set; }
		public DateTime ActiveTime { get; set; }
	}
}
