using GameComms;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

//POST to a URL creates a child resource at a server defined URL.
//PUT to a URL creates/replaces the resource in its entirety at the client defined URL.
////PATCH to a URL updates part of the resource at that client defined URL.
//POST creates a child resource, so POST to /items creates a resources that lives under the /items resource. Eg. /items/1. Sending the same post packet twice will create two resources.
//PUT is for creating or replacing a resource at a URL known by the client.
//Therefore: PUT is only a candidate for CREATE where the client already knows the url before the resource is created.Eg. / blogs / nigel / entry / when_to_use_post_vs_put as the title is used as the resource key
//PUT replaces the resource at the known url if it already exists, so sending the same request twice has no effect. In other words, calls to PUT are idempotent.

namespace GameServer.Controllers
{
	[Route("api/[controller]")]
	public class ChatController : Controller
	{
		[HttpGet("{roomId}")]
		public JsonResult Get(int roomId)
		{
			string sql;
			JsonResult query;
			ChatMessageResponse response = new ChatMessageResponse();

			Response r;
			ChatRoom room = GetChatRoom(roomId, out r);
			if(room == null)
            {
				response.Message = r.Message;
				response.Success = r.Success;
				return Json(r);
            }

			sql = $"SELECT * FROM Chat.ChatMessages WHERE roomId={roomId}";
			query = SqlConnector.Query(sql);

			response.ChatMessages = JsonConvert.DeserializeObject<ChatMessage[]>((string)query.Value);

			return Json(response);
		}

		[HttpPost]
		public JsonResult Post([FromBody] ChatMessagePostRequest request)
		{
			ChatMessageResponse response = new ChatMessageResponse();

			int roomId = request.ChatRoomId;
			int userId = request.SenderUserId;

			Response roomResponse;
			ChatRoom room = GetChatRoom(roomId, out roomResponse);
			if(room == null)
			{
				response.Message = roomResponse.Message;
				response.Success = roomResponse.Success;
				return Json(response);
            }

			ChatUserResponse chatUserResponse;
			ChatUser user = GetChatUser(userId, roomId, true, out chatUserResponse);
			if (user == null)
			{
				response.Message = chatUserResponse.Message;
				response.Success = chatUserResponse.Success;
				return Json(response);
			}

			PostChatMessage(user, request.Message, out response);

			return Json(response);
		}

		[HttpPut]
		public JsonResult Put([FromBody] JoinChatRoomRequest request)
		{
			ChatUserResponse response;
			ChatUser chatUser = JoinChatRoom(request.SenderUserId, request.ChatRoomId, out response);
			response.ChatRoomUser = chatUser;
			
			return Json(response);
		}

		public static ChatUser JoinChatRoom(int userId, int roomId, out ChatUserResponse response)
        {
			string sql;
			JsonResult query;
			response = new ChatUserResponse();

			Response r;
			ChatRoom room = GetChatRoom(roomId, out r);
			if(room == null)
            {
				response.Success = r.Success;
				response.Message = r.Message;
				return null;
            }

			ChatUser existingChatUser = GetChatUser(userId, roomId, true, out response);
			if (existingChatUser != null)
			{
				sql = $"UPDATE Chat.ChatUsers SET activeTime = SYSDATETIME() WHERE id={existingChatUser.Id} SELECT * FROM Chat.ChatUsers WHERE id={existingChatUser.Id}";
				query = SqlConnector.Query(sql);
				ChatUser updatedUser = JsonConvert.DeserializeObject<ChatUser>((string)query.Value);
				response.ChatRoomUser = updatedUser;
				response.Message = "Returning existing user";
				response.Success = true;

				return existingChatUser;
			}

			sql = $"INSERT INTO Chat.ChatUsers VALUES ({userId}, {roomId}, SYSDATETIME()) SELECT * FROM Chat.ChatUsers WHERE id=SCOPE_IDENTITY()";
			query = SqlConnector.Query(sql);
			ChatUser newChatUser = JsonConvert.DeserializeObject<ChatUser>((string)query.Value);

			response.Message = "New chatuser created";
			response.ChatRoomUser = newChatUser;
			response.Success = true;

			return newChatUser;
		}

		public static ChatRoom GetChatRoom(int roomId, out Response response)
		{
			response = new Response();

			string sql = $"SELECT * FROM Chat.ChatRooms WHERE id={roomId}";
			JsonResult query = SqlConnector.Query(sql);

			var result = JsonConvert.DeserializeObject<ChatRoom>((string)query.Value);

			if (result == null)
			{
				response.Success = false;
				response.Message = $"Chat room {roomId} does not exist";
				return null;
			}

			response.Success = true;
			response.Message = "success";

			return result;
		}

		public static ChatUser GetChatUser(int userId, int roomId, bool updateActiveTime, out ChatUserResponse response)
		{
			response = new ChatUserResponse();
			ChatUser chatUser;

			string sql = $"SELECT * FROM Chat.ChatUsers WHERE userId={userId} AND roomId={roomId}";
			JsonResult query = SqlConnector.Query(sql);

			if (query == null || (string)query.Value == "")
			{
				response.Success = false;
				response.Message = $"ChatUser does not exist for user {userId} in room {roomId}";
				return null;
			}

			chatUser = JsonConvert.DeserializeObject<ChatUser>((string)query.Value);

			if (updateActiveTime)
            {
				sql = $"UPDATE Chat.ChatUsers SET activeTime = SYSDATETIME() WHERE id={chatUser.Id} SELECT * FROM Chat.ChatUsers WHERE id={chatUser.Id}";
				query = SqlConnector.Query(sql);
				chatUser = JsonConvert.DeserializeObject<ChatUser>((string)query.Value);
			}
			
			response.ChatRoomUser = chatUser;
			response.Success = true;
			response.Message = "success";

			return chatUser;
		}

		public static ChatMessage PostChatMessage(ChatUser user, string message, out ChatMessageResponse response)
		{
			response = new ChatMessageResponse();
			ChatMessage chatMessage;

			//ChatUserResponse r;
			//ChatUser existingChatUser = GetChatUser(user.UserId, user.RoomId, true, out r);
			//if(existingChatUser == null)
   //         {
			//	response.Message = r.Message;
			//	response.Success = r.Success;
			//	return null;
   //         }

			string sql = $"INSERT INTO Chat.ChatMessages VALUES ({user.UserId}, {user.RoomId}, '{message}', SYSDATETIME()) SELECT * FROM Chat.ChatMessages WHERE id=SCOPE_IDENTITY()";

			JsonResult query = SqlConnector.Query(sql);

			if ((string)query.Value == "")
			{
				throw new Exception("failed to send message");
			}

			chatMessage = JsonConvert.DeserializeObject<ChatMessage>((string)query.Value);
			response.ChatMessages = new ChatMessage[1] { chatMessage };
			response.Success = true;
			response.Message = "success";

			return chatMessage;
		}
	}
}