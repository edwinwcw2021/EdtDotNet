using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DataLayer.DAL
{
	public class DalBookInventory : BaseDal
	{
		public DalBookInventory(EdtBookingContext context) : base(context)
		{
		}

#nullable disable
		public async Task<BookInventory?> GetInventoryById(int BookInventoryId)
		{
			var ret = await this._context.BookInventories.Where(x => x.BookInventoryId == BookInventoryId).ToListAsync();
			return ret.FirstOrDefault();
		}

		public async Task<List<vwAvailableBook>> GetAvailableInventoryByISBN(string ISBN)
		{
			//return await this._context.vwAvailableBooks.Where(x => x.ISBN == ISBN && x.isBorrowed == false).OrderBy(x=>x.CopiesNumber).ToListAsync();
			return await this._context.vwAvailableBooks.Where(x => x.ISBN == ISBN).OrderBy(x => x.CopiesNumber).ToListAsync();
		}

		public async Task<bool> IsInventoryExists(int BookInventoryId)
		{
			var ret = await GetInventoryById(BookInventoryId);
			return (ret!=null);
		}

		public async Task<List<vwAvailableBook>> GetAllBorrowedInventory()
		{
			return await this._context.vwAvailableBooks.Where(x => x.isBorrowed== true).OrderBy(x => x.CopiesNumber).ToListAsync();
		}

	}
}
