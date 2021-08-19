using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Controllers
{
	[Route("api/[controller]")]
	public class PulseController : Controller
	{
		[HttpGet]
		public JsonResult Get()
		{
			return Json(true);
		}
	}
}