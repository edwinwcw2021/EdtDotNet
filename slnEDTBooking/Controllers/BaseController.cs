using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace slnEDTBooking.Controllers
{
	public class BaseController : Controller
	{
		protected IConfiguration _configuration;
		protected readonly EdtBookingContext _context;

		public BaseController(IConfiguration configuration, EdtBookingContext context)
		{
			_configuration = configuration;
			_context = context;
		}
	}
}
