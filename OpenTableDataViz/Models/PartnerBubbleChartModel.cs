using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Models
{
	public class PartnerBubbleChartModel
	{
		public string PartnerName { get; set; }

		public long ReservationCount { get; set; }

		public string Location { get; set; }

		public long LocationCount { get; set; }
	}
}