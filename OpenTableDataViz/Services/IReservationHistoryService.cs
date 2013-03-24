using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	using OpenTableDataViz.Models;

	public interface IReservationHistoryService
	{
		List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate);
	}

	public class ReservationHistoryService : IReservationHistoryService
	{
		private IMongoDBService mongo;

		private IAppConfiguration appConfiguration;

		public ReservationHistoryService(IMongoDBService mongo, IAppConfiguration appConfiguration)
		{
			this.mongo = mongo;
			this.appConfiguration = appConfiguration;
		}

		public List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate)
		{
			//var mongo = new Mongo();

			//var collection = mongo.GetCollection<List<ReservationHistoryModel>>("ReservationCount");

			// get top 100 restaurants for each day between start and endDate ordered b

			return new List<ReservationHistoryModel>();
		}
	}
}
