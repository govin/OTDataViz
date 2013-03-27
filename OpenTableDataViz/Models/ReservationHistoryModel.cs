namespace OpenTableDataViz.Models
{
	using MongoDB.Bson;

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
