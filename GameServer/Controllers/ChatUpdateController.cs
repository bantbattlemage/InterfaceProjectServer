using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using GameComms;
using Newtonsoft.Json;
using System;

namespace GameServer.Controllers
{
	[Route("api/[controller]")]
	public class ChatUpdateController : Controller
	{

		[HttpPut]
		public JsonResult Put([FromBody] ChatMessageReadRequest request)
		{
			string sql;
			JsonResult query;
			ChatMessageResponse response = new ChatMessageResponse();

			Response r;
			ChatRoom room = ChatController.GetChatRoom(request.ChatRoomId, out r);
			if (room == null)
			{
				response.Message = r.Message;
				response.Success = r.Success;
				return Json(r);
			}

			//ChatUserResponse chatUserResponse;
			//ChatUser user = ChatController.GetChatUser(request.SenderUserId, room.Id, false, out chatUserResponse);
			//if (user == null)
			//{
			//	ChatController.JoinChatRoom(request.SenderUserId, room.Id, out chatUserResponse);
			//	response.Message = chatUserResponse.Message;
			//	response.Success = chatUserResponse.Success;
			//}

			DateTime time = request.LastMessageRead.AddSeconds(0.1d);

			sql = $"SELECT * FROM Chat.ChatMessages WHERE roomId={room.Id} AND DATEDIFF(second, timestamp, {SqlConnector.SqlString(time)}) < 0";
			query = SqlConnector.Query(sql);

			if(query == null || (string)query.Value == "")
			{
				response.ChatMessages = new ChatMessage[0];
			}
			else
			{
				try
				{
					response.ChatMessages = JsonConvert.DeserializeObject<ChatMessage[]>((string)query.Value);
				}
				catch
				{
					response.ChatMessages = new ChatMessage[] { JsonConvert.DeserializeObject<ChatMessage>((string)query.Value) };
				}
			}

			response.Success = true;
			response.Message = $"{response.ChatMessages.Length} new messages";

			return Json(response);
		}
	}
}