using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Models
{
	public class ResoCountBubbleChartModel
	{
		public string RName { get; set; }

		public long ReservationCount { get; set; }

		public string Location { get; set; }

		//public string LocationId { get; set; }
	}
}