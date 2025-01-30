using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using static NLog.LayoutRenderers.Wrappers.ReplaceLayoutRendererWrapper;
using System.Buffers.Text;
using DataLayer.BLL;
using DataLayer;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
namespace slnEDTBooking.Controllers
{
	/* 
	Based on the limitations of the web service in the production Nginx web server, we will replace the proxy setup. 
	Different endpoints will be mapped for development and production environments.
	*/
#if DEBUG
	[Route("/api/[controller]")]
#else
	[Route("[controller]")]
#endif

	[ApiController]
	public class BookLibraryController : BaseController
	{
		public BookLibraryController(IConfiguration configuration, EdtBookingContext context) : base(configuration, context) { }

		[HttpGet("GetBooksByKeyWords")]
		[ProducesResponseType(200, Type = typeof(Book))]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetBooksByKeyWords(string keyword)
		{
			if (!string.IsNullOrEmpty(keyword))
			{
				if (keyword.Length < 3)
				{
					Common.ErrLog(string.Format("The minimum length for keywords is three characters. {0}", keyword));
					return Problem(
							statusCode: 400,
							type: "Bad Request",
							title: "Bad Request",
							detail: "The minimum length for keywords is three characters."
					);
				}
				try
				{
					BLLBooks bk = new BLLBooks(this._context, this._configuration);
					var result = await bk.GetBooksByKeyWords(keyword);
					Common.AppLog(result.Count().ToString());
					return Json(result);
				}
				catch (Exception ex)
				{
					Common.ErrLog(ex.ToString());
					return Problem(
							statusCode: 500,
							type: "Internal Server Error",
							title: "Internal Server Error",
							detail: "System error. Please contact the IT department."
					);
				}
			}
			else
			{
				Common.ErrLog(string.Format("The system does not allow null or empty keywords to be entered. {0}", keyword));
				return Problem(
						statusCode: 400,
						type: "Bad Request",
						title: "Bad Request",
						detail: "The system does not allow null or empty keywords to be entered."
				);
			}
		}

		[HttpPost("BorrowAvailableBook")]
		[ProducesResponseType(200, Type = typeof(BorrowHistory))]
		[ProducesResponseType(404)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> BorrowAvailableBook([FromForm] BorrowInput borrow)
		{
			try
			{
				BLLBorrow br = new BLLBorrow(this._context, this._configuration);
				var result = await br.BorrowBook(borrow.BookInventoryId, borrow.UserId);
				if (result.BorrowId == -1)
				{
					Common.ErrLog(string.Format("Data integrity issue due to concurrent updates or dirty reads. {0}", borrow));
					return Problem(
							statusCode: 409,
							type: "Conflict",
							title: "Data integrity issue due to concurrent updates or dirty reads.",
							detail: "The resource has been booked by someone else."
					);
				}
				if (result.BorrowId == -2)
				{
					Common.ErrLog(string.Format("The number of books borrowed per user has reached the limit. {0}", borrow));
					return Problem(
							statusCode: 409,
							type: "User Limit",
							title: "The number of books borrowed per user has reached the limit.",
							detail: "The number of books borrowed per user has reached the limit."
					);
				}
				if (result.BorrowId == -3)
				{
					Common.ErrLog(string.Format("Inventory Not found {0}", borrow));
					return Problem(
							statusCode: 404,
							type: "Not Found",
							title: "Inventory Not found",
							detail: "Inventory Not found"
					);
				}
				if (result.BorrowId == -4)
				{
					Common.ErrLog(string.Format("User Not found {0}", borrow));
					return Problem(
							statusCode: 404,
							type: "Not Found",
							title: "User Not found",
							detail: "User Not found"
					);
				}
				return Json(result);
			}
			catch (Exception ex)
			{
				Common.ErrLog(ex.ToString());
				return Problem(
						statusCode: 500,
						type: "System Errors",
						title: "System Errors handling",
						detail: "System error. Please contact the IT department."
				);
			}
		}

		[HttpPut("BorrowBookReturn")]
		[ProducesResponseType(200, Type = typeof(BorrowHistory))]
		[ProducesResponseType(404)]
		[ProducesResponseType(409)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> BorrowBookReturn([FromForm] string sBookInventoryId)
		{
			try
			{
				int BookInventoryId = -1;
				if (!int.TryParse(sBookInventoryId, out BookInventoryId))
				{
					Common.ErrLog(string.Format("BookInventoryId must be Integer {0}", sBookInventoryId));
					return Problem(
							statusCode: 400,
							type: "Bad Request",
							title: "BookInventoryId must be Integer",
							detail: "Book Inventory Id must be Integer"
					);
				}

				BLLBorrow br = new BLLBorrow(this._context, this._configuration);
				var result = await br.BookReturn(BookInventoryId);
				if (result.BorrowId == -3)
				{
					Common.ErrLog(string.Format("Inventory Not found {0}", sBookInventoryId));
					return Problem(
							statusCode: 404,
							type: "Not Found ",
							title: "Inventory Not found",
							detail: "Inventory Not found"
					);
				}
				if (result.BorrowId == -1)
				{
					Common.ErrLog(string.Format("Borrow record not found—either the borrow ID is invalid or the book has already been returned. {0}", sBookInventoryId));
					return Problem(
							statusCode: 409,
							type: "Conflic",
							title: "borrow ID is invalid",
							detail: "Borrow record not found—either the borrow ID is invalid or the book has already been returned."
					);
				}
				return Json(result);
			}
			catch (Exception ex)
			{
				Common.ErrLog(ex.ToString());
				return Problem(
						statusCode: 500,
						type: "Internal Server Error",
						title: "Internal Server Error",
						detail: "System error. Please contact the IT department."
				);
			}

		}

		//[HttpGet("GetUserBorrowHistory")]
		//[ProducesResponseType(200, Type = typeof(BorrowHistory))]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		//[ProducesResponseType(500)]
		//public async Task<IActionResult> GetUserBorrowHistory(string sUserId)
		//{
		//	try
		//	{
		//		int UserId = -1;
		//		if (!int.TryParse(sUserId, out UserId))
		//		{
		//			Common.ErrLog(string.Format("UserId must be Integer. {0}", sUserId));
		//			return Problem(
		//					statusCode: 400,
		//					type: "Bad Request",
		//					title: "UserId must be Integer",
		//					detail: "UserId must be Integer"
		//			);
		//		}
		//		BLLBorrow br = new BLLBorrow(this._context, this._configuration);
		//		var result = await br.GetUserBorrowHistory(UserId);
		//		if (result.RetMsgId == -4)
		//		{
		//			Common.ErrLog(string.Format("User Not found. {0}", sUserId));
		//			return Problem(
		//					statusCode: 404,
		//					type: "Not Found",
		//					title: "User Not found",
		//					detail: "User Not found"
		//			);
		//		}
		//		return Json(result.BorrowHist);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.ErrLog(ex.ToString());
		//		return Problem(
		//				statusCode: 500,
		//				type: "Internal Server Error",
		//				title: "Internal Server Error",
		//				detail: "System error. Please contact the IT department."
		//		);
		//	}
		//}

		//[HttpGet("GetUserBorrowButNotReturnItems")]
		//[ProducesResponseType(200, Type = typeof(BorrowHistory))]
		//[ProducesResponseType(400)]
		//[ProducesResponseType(404)]
		//[ProducesResponseType(500)]
		//public async Task<IActionResult> GetUserBorrowButNotReturnItems(string UserId)
		//{
		//	try
		//	{
		//		int intUserId = -1;
		//		if (!int.TryParse(UserId, out intUserId))
		//		{
		//			Common.ErrLog(string.Format("UserId must be Integer. {0}", UserId));
		//			return Problem(
		//					statusCode: 400,
		//					type: "Bad Request",
		//					title: "UserId must be Integer",
		//					detail: "UserId must be Integer"
		//			);
		//		}
		//		BLLBorrow br = new BLLBorrow(this._context, this._configuration);
		//		var result = await br.GetUserBorrowButNotReturnItems(intUserId);
		//		if (result.RetMsgId == -4)
		//		{
		//			Common.ErrLog(string.Format("User Not found {0}", UserId));
		//			return Problem(
		//					statusCode: 404,
		//					type: "Not found",
		//					title: "User Not found",
		//					detail: "User Not found"
		//			);
		//		}
		//		return Json(result.BorrowHist);
		//	}
		//	catch (Exception ex)
		//	{
		//		Common.ErrLog(ex.ToString());
		//		return Problem(
		//				statusCode: 500,
		//				type: "Internal Server Error",
		//				title: "Internal Server Error",
		//				detail: "System error. Please contact the IT department."
		//		);
		//	}
		//}

		[HttpGet("GetAllUsers")]
		[ProducesResponseType(200, Type = typeof(User))]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				BLLUser usr = new BLLUser(this._context, this._configuration);
				var ret = await usr.GetAllUser();
				return Json(ret);
			}
			catch (Exception ex)
			{
				Common.ErrLog(ex.ToString());
				return Problem(
						statusCode: 500,
						type: "Internal Server Error",
						title: "Internal Server Error",
						detail: "System error. Please contact the IT department."
				);
			}
		}

		[HttpGet("GetAvailableInventoryByISBN")]
		[ProducesResponseType(200, Type = typeof(vwAvailableBook))]
		[ProducesResponseType(400)]
		[ProducesResponseType(404)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetAvailableInventoryByISBN(string isbn)
		{
			try
			{
				if (string.IsNullOrEmpty(isbn))
				{
					return Problem(
							statusCode: 400,
							type: "Bad Request",
							title: "Bad Request",
							detail: "ISBN cannot be empty."
					);
				}
				BLLBorrow borrow = new BLLBorrow(this._context, this._configuration);
				var ret = await borrow.GetAvailableInventoryByISBN(isbn);
				return Json(ret);
			}
			catch (Exception ex)
			{
				Common.ErrLog(ex.ToString());
				return Problem(
						statusCode: 500,
						type: "Internal Server Error",
						title: "Internal Server Error",
						detail: "System error. Please contact the IT department."
				);
			}
		}

		[HttpGet("GetAllBorrowedInventory")]
		[ProducesResponseType(200, Type = typeof(vwAvailableBook))]
		[ProducesResponseType(400)]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetAllBorrowedInventory() 
		{
			try
			{
				BLLBorrow borrow = new BLLBorrow(this._context, this._configuration);
				var ret = await borrow.GetAllBorrowedInventory();
				return Json(ret);
			}
			catch (Exception ex)
			{
				Common.ErrLog(ex.ToString());
				return Problem(
						statusCode: 500,
						type: "Internal Server Error",
						title: "Internal Server Error",
						detail: "System error. Please contact the IT department."
				);
			}
		}
	}
}
