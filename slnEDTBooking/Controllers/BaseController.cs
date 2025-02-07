using DataLayer.Models;
using DataLayer;
using Microsoft.AspNetCore.Mvc;


namespace slnEDTBooking.Controllers
{
	public class BaseController : Controller
	{
		protected readonly EdtBookingContext _context;

		public BaseController(EdtBookingContext context)
		{
			_context = context;
		}

		protected IActionResult HandleError(int code, string details = "")
		{
			Common.Instance.ErrLog(details);
			var status = Common.Instance.GetStatusInfoFromCode(code);
			return Problem(type: status.type,
				title: status.title,
				detail: details,
				statusCode: code 
			);
		}
	}
}
