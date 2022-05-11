using Microsoft.AspNetCore.Mvc;

namespace Foxhole_Server.Controllers
{
	[Route("/")]
	[Route("[controller]/")]
	[Produces("application/json")]
	[ApiController]
	public class HomeController : ControllerBase
	{
		[HttpGet]
		public JsonResult Get()
		{

			return new("{}") { StatusCode = 200 };
		}
	}
}
