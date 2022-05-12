using Foxhole_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace Foxhole_Server.Controllers
{
	[Route("[controller]/{id}/")]
	[ApiController]
	public class BurrowController : ControllerBase
	{
		[HttpGet]
		public JsonResult Get(string ID)
		{
			Burrow? burrow;
			using (Burrow_Context schema = new(new()))
			{
				burrow = schema.Burrows.SingleOrDefault
				(
					burrow => burrow.ID ==
							ID
							.Replace('_', '/')
							.Replace('-', '+')
				);
			}
			if (burrow == null) { return new(new { message = "The burrow you requested does not exist", status = 404 }); }

			return new(burrow);
		}
	}
}
