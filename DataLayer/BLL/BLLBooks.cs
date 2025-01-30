using DataLayer.DAL;
using DataLayer.Models;
using Microsoft.Extensions.Configuration;

namespace DataLayer.BLL
{
	public class BLLBooks : BaseBLL
	{ 
		public BLLBooks(EdtBookingContext context, IConfiguration configuration) : base(context, configuration)
		{
		}

		public async Task InitCache()
		{
			BLLUser usr = new BLLUser(this._context, this._configuration);
			await usr.GetAllUser();

			DalBooks dal = new DalBooks(this._context);
			await dal.GetAllBooks();			
		}
		public async Task<List<Book>> GetBooksByKeyWords(string keywords)
		{
			DalBooks dal = new DalBooks(this._context);	
			return await dal.GetBooksByKeyWords(keywords);
		}

	}
}
