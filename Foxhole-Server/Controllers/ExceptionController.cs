using Microsoft.AspNetCore.Mvc;

namespace Foxhole_Server.Controllers
{
	[Route("[controller]/")]
	[ApiController]
	public class ExceptionController : ControllerBase
	{
		[Route("404/")]
		[HttpGet]
		public StatusCodeResult HTTP_404()
		{
			return StatusCode(404);
		}
	}
}
