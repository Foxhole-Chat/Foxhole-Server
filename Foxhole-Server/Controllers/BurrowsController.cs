using Foxhole_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Foxhole_Server.Controllers
{
	[Route("[controller]/")]
	[ApiController]
	public class BurrowsController : ControllerBase
	{
		[HttpGet]
		public JsonResult Get()
		{
			using Burrow_Context schema = new(new());

			return new(JsonConvert.SerializeObject(schema.Burrows));
		}
	}
}
