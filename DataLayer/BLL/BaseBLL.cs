using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.BLL
{
	public class BaseBLL
	{
		protected readonly EdtBookingContext _context;
		protected IConfiguration _configuration;

		public BaseBLL(EdtBookingContext context, IConfiguration configuration)
		{
			this._context = context;
			this._configuration = configuration;
		}
	}
}
