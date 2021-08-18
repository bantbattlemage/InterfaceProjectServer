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

			DateTime time = request.LastMessageRead.AddSeconds(0.1d);

			sql = $"SELECT * FROM Chat.ChatMessages WHERE roomId={request.ChatRoomId} AND DATEDIFF(second, timestamp, '{time.ToSqlTime()}') < 0";
			query = SqlConnector.Query(sql);

			response.ChatMessages = query.ExtractListFromResult<ChatMessage>();
			response.Success = true;
			response.Message = $"{response.ChatMessages.Length} new messages";

			return Json(response);
		}
	}
}