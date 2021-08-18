﻿using System;

namespace GameComms
{
	public class Response
    {
		public bool Success { get; set; }
		public string Message { get; set; }
	}

	public class LogInResponse : Response
	{
		public string AccessKey { get; set; }
		public string Username { get; set; }
		public int UserId { get; set; }
	}

	public class GetUserResponse : Response
	{
		public GameUser User { get; set; }
	}

	public class ChatJoinResponse : Response
	{
		public ChatUser AssignedChatUser { get; set; }
		public ChatMessage[] ChatMessages { get; set; }
	}

	public class ChatMessageResponse : Response
	{
		public ChatMessage[] ChatMessages { get; set; }
	}

	public class ChatUserResponse : Response
    {
		public ChatUser ChatRoomUser { get; set; }
	}
}