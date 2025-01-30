using DataLayer.DAL;
using DataLayer.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Configuration;

namespace DataLayer.BLL
{


	public class BLLBorrow : BaseBLL
	{
		public BLLBorrow(EdtBookingContext context, IConfiguration configuration) : base(context, configuration)
		{
		}
				
		public async Task<BorrowHistory> BorrowBook(int BookingInventoryId, int UserId)
		{
			DalBorrow dalBorrow = new DalBorrow(this._context);
			DalUsers dalUser = new DalUsers(this._context);
			DalBookInventory dalInventory = new DalBookInventory(this._context);

			int DaysOfBorrow = Common.GetNoOfBorrowDays(this._configuration);
			int UserBorrowLimit = Common.GetBorrowLimitPerUser(this._configuration);
			BorrowHistory bh = new BorrowHistory();

			// Check if the inventory exists.
			if (!dalInventory.IsInventoryExists(BookingInventoryId).Result)
			{
				bh.BorrowId = -3; // Using data handling for errors is better than raising exceptions.
				return bh;
			}

			// Check if the user exists.
			if (!dalUser.IsUserExists(UserId).Result)
			{
				bh.BorrowId = -4; 
				return bh;
			}

			// Check if the book is borrowed in the system.
			var checkbookIsBorrowed = dalBorrow.GetBorrowByInventory(BookingInventoryId).Result;
			if (checkbookIsBorrowed != null)
			{
				bh.BorrowId = -1; 
				return bh;			
			}

			// Check if the user has reached their borrowing limit.
			var NoOfBorrowBookPerUser = dalBorrow.GetBorrowCountByUserId(UserId).Result;
			if (NoOfBorrowBookPerUser >= UserBorrowLimit)
			{
				bh.BorrowId = -2; 
				return bh;
			}

			//Pass the validation
			DateTime DateOfBorrow = DateTime.Now;
			DateTime DateOfReturn = DateOfBorrow.AddDays(DaysOfBorrow).Date;
			bh.DateBorrow = DateOfBorrow;
			bh.BookInventoryId = BookingInventoryId;
			bh.DateExpectedReturn = DateOfReturn;
			bh.BorrowByUserId = UserId;
			
			return await dalBorrow.AddBorrow(bh);
		} 

		public async Task<BorrowHistory> BookReturn(int BookingInventoryId)
		{
			DalBookInventory dalInventory = new DalBookInventory(this._context);
			DalBorrow dalBorrow = new DalBorrow(this._context);
			BorrowHistory bh = new BorrowHistory();
			// Check if the inventory exists.
			if (!dalInventory.IsInventoryExists(BookingInventoryId).Result)
			{
				bh.BorrowId = -3; // Using data handling for errors is better than raising exceptions.
				return bh;
			}

			var current_borrow = dalBorrow.GetBorrowByInventory(BookingInventoryId).Result;
			if(current_borrow==null)
			{
				bh.BorrowId = -1;
				return bh;
			}

			//Pass the validation
			current_borrow.DateReturn = DateTime.Now;
			return await dalBorrow.UpdateBorrow(current_borrow);	
		}

		public async Task<BorrowResult> GetUserBorrowHistory(int UserId)
		{
			DalBorrow dalBorrow = new DalBorrow(this._context);
			DalUsers dalUser = new DalUsers(this._context);
			BorrowResult ret=new BorrowResult();
			
			// Check if the user exists.
			if (!dalUser.IsUserExists(UserId).Result)
			{
				ret.RetMsgId = -4;	
				return ret;
			}

			//Pass the validation
			var data = await dalBorrow.GetUserBorrowHistory(UserId);
			ret.RetMsgId = 0;
			ret.BorrowHist = data;
			return ret;

		}

		public async Task<BorrowResult> GetUserBorrowButNotReturnItems(int UserId)
		{
			DalBorrow dalBorrow = new DalBorrow(this._context);
			DalUsers dalUser = new DalUsers(this._context);
			BorrowResult ret = new BorrowResult();

			// Check if the user exists.
			if (!dalUser.IsUserExists(UserId).Result)
			{
				ret.RetMsgId = -4;
				return ret;
			}

			//Pass the validation
			var data = await dalBorrow.GetUserBorrowButNotReturnItems(UserId);
			ret.RetMsgId = 0;
			ret.BorrowHist = data;
			return ret;
		}

		public async Task<List<vwAvailableBook>> GetAvailableInventoryByISBN(string ISBN)
		{
			DalBookInventory dalInv =  new DalBookInventory(this._context);
			var data = await dalInv.GetAvailableInventoryByISBN(ISBN);
			return data;
		}

		public async Task<List<vwAvailableBook>> GetAllBorrowedInventory()
		{
			DalBookInventory dalInv = new DalBookInventory(this._context);
			var data = await dalInv.GetAllBorrowedInventory();
			return data;
		}
	}
}
