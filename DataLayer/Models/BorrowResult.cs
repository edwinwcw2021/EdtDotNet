using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
	public class BorrowResult
	{
		public int RetMsgId { get; set; }

		public List<BorrowHistory>? BorrowHist { get; set; }
	}
}
