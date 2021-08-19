using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Controllers
{
	[Route("api/[controller]")]
	public class TemplateController : Controller
	{
		[HttpGet]
		public JsonResult Get()
		{
			return Json(false);
		}

		[HttpGet("{id}")]
		public JsonResult Get(int id)
		{
			return Json(false);
		}

		[HttpPost]
		public JsonResult Post([FromBody] string value)
		{
			return Json(false);
		}

		[HttpPut("{id}")]
		public JsonResult Put(int id, [FromBody] string value)
		{
			return Json(false);
		}

		[HttpDelete("{id}")]
		public JsonResult Delete(int id)
		{
			return Json(false);
		}
	}
}