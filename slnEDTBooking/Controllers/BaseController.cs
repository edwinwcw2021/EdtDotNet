using DataLayer.Models;
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
	}
}
