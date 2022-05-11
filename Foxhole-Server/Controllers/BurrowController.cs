using Foxhole_Server.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Foxhole_Server.Controllers
{
	[Route("[controller]/{id}/")]
	[ApiController]
	public class BurrowController : ControllerBase
	{
		[HttpGet]
		public JsonResult Get(Guid ID)
		{
			using (Burrow_Context schema = new(new()))
			{

			}

			return new("{}");
		}
	}
}
