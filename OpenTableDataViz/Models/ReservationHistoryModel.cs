using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Models
{
	public class ReservationHistoryModel
	{
		public string RestaurantName { get; set; }

		public string ISOShiftDate { get; set; }

		public int ReservationCount { get; set; }

		public int AvgPartySize { get; set; }

		public string Location { get; set; }
	}
}
