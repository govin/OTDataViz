using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Models
{
	using MongoDB.Bson;

	public class RestaurantModel
	{
		public ObjectId Id { get; set; }

		public long RID { get; set; }

		public string RName { get; set; }

		public string RestaurantType { get; set; }

		public int RestStateID { get; set; }

		public string ExternalURL { get; set; }

		public double Latitude { get; set; }

		public double Longitude { get; set; }

		public string MenuURL { get; set; }

		public int LanguageID { get; set; }

		public string FacebookURL { get; set; }

		public int ZIP { get; set; }

		public string Country { get; set; }

		public int NeighborhoodID { get; set; }

		public string Nbhoodname { get; set; }

		public int MetroAreaID { get; set; }

		public string MetroAreaName { get; set; }

		public string CuisineType { get; set; }

		public string DiningStyle { get; set; }


	}
}