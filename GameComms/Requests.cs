using System;

namespace GameComms
{
	public class Request
    {

    }

	public class LogInRequest : Request
	{
		public string Username { get; set; }
		public string Password { get; set; }
		public string Email { get; set; }
		public bool NewRegistration { get; set; }
	}

	public class GetUserRequest : Request
	{
		public int UserId { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
	}

	public class JoinChatRoomRequest : Request
    {
		public int SenderUserId { get; set; }
		public int ChatRoomId { get; set; }
		public string Username { get; set; }
    }

	public class ChatMessagePostRequest : Request
	{
		public int SenderUserId { get; set; }
		public int ChatRoomId { get; set; }
		public string Username { get; set; }
		public string Message { get; set; }
		public string TimeStamp { get; set; }
	}

	public class ChatMessageReadRequest : Request
	{
		public int SenderUserId { get; set; }
		public int ChatRoomId { get; set; }
		public DateTime LastMessageRead { get; set; }
	}

	public class ChatMessageReadRequests : Request
	{
		public ChatMessageReadRequest[] Requests;
	}
}
