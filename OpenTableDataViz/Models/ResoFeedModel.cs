using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace OpenTableDataViz.Models
{
	[DataContract]
	public class ResoFeedModel
	{
		[DataMember]
		public string Title { get; set; }

		[DataMember]
		public string Href_self { get; set; }

		[DataMember]
		public string Href_prev { get; set; }

		[DataMember]
		public long Max_Resid { get; set; }

		[DataMember]
		public DateTime Max_DateMadeUtc { get; set; }

		[DataMember]
		public List<Reservation> Reservations { get; set; }
	}

	public class Reservation
	{
		[DataMember]
		public long Resid { get; set; }

		[DataMember]
		public DateTime DateMadeUtc { get; set; }

		[DataMember]
		public DateTime ShiftDateTime { get; set; }

		[DataMember]
		public int Partysize { get; set; }

		[DataMember]
		public string Billingtype { get; set; }

		[DataMember]
		public long Rid { get; set; }

		[DataMember]
		public string RestaurantName { get; set; }

		[DataMember]
		public double Latitude { get; set; }

		[DataMember]
		public double Longitude { get; set; }

		[DataMember]
		public int Partnerid { get; set; }

		[DataMember]
		public string Partnername { get; set; }
	}
}