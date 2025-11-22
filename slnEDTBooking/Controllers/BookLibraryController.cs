using DataLayer;
using DataLayer.BLL;
using DataLayer.Models;
using Microsoft.AspNetCore.Mvc;
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
		public BookLibraryController(EdtBookingContext context) : base(context) { }

		[HttpGet("book/search")]
		[ProducesResponseType(200, Type = typeof(Book))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> GetBooksByKeyWords([FromQuery] string? keyword)
		{
				try
				{
					BLLBooks bk = new BLLBooks(this._context);
					var result = await bk.GetBooksByKeyWords(keyword);
					Common.Instance.AppLog(result.Count().ToString());
					return Json(result);
				}
				catch (Exception ex)
				{
					return HandleError(500, "System error. Please contact the IT department.");
				}
		}

		[HttpPost("book")]
		[ProducesResponseType(200, Type = typeof(BorrowHistory))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> BorrowAvailableBook([FromForm] BorrowInput borrow)
		{
			try
			{
				BLLBorrow br = new BLLBorrow(this._context);
				var result = await br.BorrowBook(borrow.BookInventoryId, borrow.UserId);
				if (result.BorrowId == -1)
				{
					return HandleError(409, "The resource has been booked by someone else.");
				}
				if (result.BorrowId == -2)
				{
					return HandleError(409, $"The number of books borrowed per user has reached the limit.");
				}
				if (result.BorrowId == -3)
				{
					return HandleError(404, $"Inventory Not found");
				}
				if (result.BorrowId == -4)
				{
					return HandleError(404, $"User Not found");
				}
				return Json(result);
			}
			catch (Exception ex)
			{
				return HandleError(500, $"System error. Please contact the IT department.");
			}
		}

		[HttpPut("book/{sBookInventoryId}")]
		[ProducesResponseType(200, Type = typeof(BorrowHistory))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]

		public async Task<IActionResult> BorrowBookReturn(string sBookInventoryId)
		{
			try
			{
				int BookInventoryId = -1;
				if (!int.TryParse(sBookInventoryId, out BookInventoryId))
				{
					return HandleError(400, $"Book Inventory Id must be Integer");
				}

				BLLBorrow br = new BLLBorrow(this._context);
				var result = await br.BookReturn(BookInventoryId);
				if (result.BorrowId == -3)
				{
					return HandleError(404, $"Inventory Not found ");
				}
				if (result.BorrowId == -1)
				{
					return HandleError(409, $"Borrow record not found—either the borrow ID is invalid or the book has already been returned.");
				}
				return Json(result);
			}
			catch (Exception ex)
			{
				return HandleError(500, $"System error. Please contact the IT department.");
			}
		}

		

		[HttpGet("users")]
		[ProducesResponseType(200, Type = typeof(User))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> GetAllUsers()
		{
			try
			{
				BLLUser usr = new BLLUser(this._context);
				var ret = await usr.GetAllUser();
				return Json(ret);
			}
			catch (Exception ex)
			{
				return HandleError(500, $"System error. Please contact the IT department.");
			}
		}

		[HttpGet("book/{isbn}")]
		[ProducesResponseType(200, Type = typeof(vwAvailableBook))]
		[ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> GetAvailableInventoryByISBN(string isbn)
		{
			try
			{
				if (string.IsNullOrEmpty(isbn))
				{
					return HandleError(400, $"ISBN cannot be empty.");
				}
				BLLBorrow borrow = new BLLBorrow(this._context);
				var ret = await borrow.GetAvailableInventoryByISBN(isbn);
				return Json(ret);
			}
			catch (Exception ex)
			{
				return HandleError(500, $"System error. Please contact the IT department.");
			}
		}

		[HttpGet("book/all")]
		[ProducesResponseType(200, Type = typeof(vwAvailableBook))]
		[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ProblemDetails))]
		[ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ProblemDetails))]
		public async Task<IActionResult> GetAllBorrowedInventory() 
		{
			try
			{
				BLLBorrow borrow = new BLLBorrow(this._context);
				var ret = await borrow.GetAllBorrowedInventory();
				return Json(ret);
			}
			catch (Exception ex)
			{
				return HandleError(500, $"System error. Please contact the IT department.");
			}
		}
	}
}
