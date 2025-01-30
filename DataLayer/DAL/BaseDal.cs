using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DAL
{
	public class BaseDal
	{
		protected readonly EdtBookingContext _context;

		public BaseDal(EdtBookingContext context)
		{
			_context = context;
		}

	}
}
