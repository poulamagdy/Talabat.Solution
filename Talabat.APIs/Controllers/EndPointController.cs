using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Controllers
{
	[Route("error/{Code}")]
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = true)]
	public class EndPointController : ControllerBase
	{
		public ActionResult Error(int code)
			=> NotFound(new ApiResponse(code));
	}
}
