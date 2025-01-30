using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace DataLayer.DAL
{
	public class DalBorrow : BaseDal
	{
		public DalBorrow(EdtBookingContext context) : base(context) { }

		public async Task<BorrowHistory> AddBorrow(BorrowHistory borrow)
		{
			await this._context.BorrowHistories.AddAsync(borrow);
			await this._context.SaveChangesAsync();
			return borrow;
		}

		public async Task<BorrowHistory> UpdateBorrow(BorrowHistory borrow)
		{
			this._context.BorrowHistories.Update(borrow);
			await this._context.SaveChangesAsync();
			return borrow;
		}

#nullable disable
		public async Task<BorrowHistory> GetBorrowByInventory(int BookInventoryId)
		{
			var query = from item in this._context.BorrowHistories
									where item.DateReturn == null && item.BookInventoryId == BookInventoryId
									select item;

			var ret = await query.ToListAsync();
			return ret.FirstOrDefault();
		}

		public async Task<int> GetBorrowCountByUserId(int UserId)
		{
			var query = from item in this._context.BorrowHistories
									where item.DateReturn == null && item.BorrowByUserId == UserId
									select item;

			var ret = await query.ToListAsync();
			return ret.Count();
		}


		public async Task<List<BorrowHistory>> GetUserBorrowHistory(int UserId)
		{
			var query = from item in this._context.BorrowHistories
									where item.BorrowByUserId == UserId
									select item;

			return await query.ToListAsync();
		}

#pragma warning disable 1998
		public async Task<List<BorrowHistory>> GetUserBorrowButNotReturnItems(int UserId)
		{
			var query = GetUserBorrowHistory(UserId).Result.Where(x=>x.DateReturn==null).ToList();
			return query;
		}
#pragma warning restore 1998
	}
}
