using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DAL
{
	public class DalUsers : BaseDal
	{
		public DalUsers(EdtBookingContext context) : base(context)
		{
		}

		public async Task<List<User>> GetAllUser()
		{
			List<User> ret = Cache.GetCache<List<User>>(Cache.CacheName.Users);
			if (ret == null)
			{
				await RefreshCache();
				ret = Cache.GetCache<List<User>>(Cache.CacheName.Users);
			}
			return ret;
		}

		public async Task<User?> GetUserById(int UserId)
		{
			var ret = await this.GetAllUser();
			ret = ret.Where(x => x.UserId == UserId).ToList();
			return ret.FirstOrDefault();
		}

		public async Task<bool> IsUserExists(int UserId)
		{
			var ret = await GetUserById(UserId);
			return (ret != null);
		}

		private async Task RefreshCache()
		{
			List<User> ret = new List<User>();
			try
			{
				ret = await this._context.Users.ToListAsync();
				var list = Common.CloneRecClass(ret);  // Remove dependance.
				Cache.SetCache(Cache.CacheName.Users, ret);
			}
			catch (Exception ex)
			{
				Common.ErrLog(ex.ToString());
			}
		}
	}
}
