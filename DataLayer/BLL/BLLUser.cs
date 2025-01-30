using DataLayer.DAL;
using DataLayer.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.BLL
{
	public class BLLUser : BaseBLL
	{
		public BLLUser(EdtBookingContext context, IConfiguration configuration) : base(context, configuration) { }

		public async Task<List<User>?> GetAllUser()
		{
			DalUsers usr = new DalUsers(this._context);
			return await usr.GetAllUser();
		}
	}
}
