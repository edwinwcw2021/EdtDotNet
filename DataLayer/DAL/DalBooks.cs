using DataLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace DataLayer.DAL
{
	public class DalBooks : BaseDal
	{

		public DalBooks(EdtBookingContext context) : base(context) { }

		public async Task<List<Book>> GetAllBooks()
		{
			List<Book> ret = Cache.Instance.GetCache<List<Book>>(Cache.CacheName.Books);
			if (ret == null)
			{
				await RefreshCache();
				ret = Cache.Instance.GetCache<List<Book>>(Cache.CacheName.Books);
			}
			return ret;
		}

#nullable disable   // Bypass unnecessary warnings.
		public async Task<List<Book>> GetBooksByKeyWords(string keywords)
		{
			Common.Instance.AppLog("start searh " + keywords);
			try
			{				
				var allBooks = await this.GetAllBooks();

				return (allBooks.Where(
					x => x.BookTitle.Contains(keywords)
					 || x.BookAuthor.Contains(keywords)
					 || x.Publisher.Contains(keywords)
					 || x.YearOfPublic.Contains(keywords)
					 || x.ISBN.Contains(keywords)
					 )).OrderBy(x => x.ISBN).Take(100000).ToList();  // Limit to the first 10,000 records to optimize resource usage.
				;
			}
			finally
			{
				Common.Instance.AppLog("end searh " + keywords);
			}
		}

		private async Task RefreshCache()
		{
			List<Book> ret = new List<Book>();
			try
			{
				ret = await this._context.Books.ToListAsync();
				var list = Common.Instance.CloneRecClass(ret);  // Remove dependance.
				Cache.Instance.SetCache(Cache.CacheName.Books, ret);
			}
			catch (Exception ex)
			{
				Common.Instance.ErrLog(ex.ToString());
			}
		}
#nullable enable

	}
}
