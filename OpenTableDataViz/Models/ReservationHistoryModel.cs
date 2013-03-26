using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Models
{
	using MongoDB.Bson;
	using MongoDB.Bson.Serialization.Attributes;
	using MongoDB.Bson.Serialization.IdGenerators;

	public class ReservationHistoryModel
	{
		public ObjectId Id { get; set; }

		public long Rid { get; set; }

		public string ShiftDate { get; set; }

		public long ResoCount { get; set; }

		public long AvgPartySize { get; set; }

		//public string Location { get; set; }

		//public string RestaurantName { get; set; }
	}
}
