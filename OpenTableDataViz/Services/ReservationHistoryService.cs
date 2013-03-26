namespace OpenTableDataViz.Services
{
	using System.Collections.Generic;

	using MongoDB.Driver;

	using OpenTableDataViz.Models;

	using System.Linq;

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
			//var collection = mongo.GetCollection<ReservationHistoryModel>("ReservationCount");

			var server = MongoServer.Create(this.appConfiguration.ConnectionStringDataVizDB);
			var dataViz = server.GetDatabase("DataViz");
			var collection = dataViz.GetCollection<ReservationHistoryModel>("ReservationCount");

			var masterDict = new Dictionary<string, Dictionary<long, List<ReservationHistoryModel>>>();
			var history = new List<ReservationHistoryModel>();
			foreach (var matchingPost in collection.FindAll())
			{
				var historyModel = new ReservationHistoryModel()
					{
						AvgPartySize = matchingPost.AvgPartySize,
						ShiftDate = matchingPost.ShiftDate, 
						//Location = "San Francisco", 
						ResoCount = matchingPost.ResoCount,
						//RestaurantName = matchingPost.RestaurantName,
						Rid = matchingPost.Rid
					};
				//var isoShiftDate = matchingPost.ShiftDate;
				//var rid = matchingPost.Rid;
				//if (masterDict.ContainsKey(isoShiftDate))
				//{
				//	var subDict = masterDict[isoShiftDate];
				//	if (subDict.ContainsKey(rid))
				//	{
				//		var list = subDict[rid];
				//		list.Add(historyModel);
				//	}
				//	else
				//	{
				//		var list = new List<ReservationHistoryModel> { historyModel };
				//		subDict.Add(rid, list);
				//	}
				//}
				//else
				//{
				//	var subDict = new Dictionary<long, List<ReservationHistoryModel>>();
				//	var list = new List<ReservationHistoryModel> { historyModel };
				//	subDict.Add(rid, list);
				//	masterDict.Add(isoShiftDate, subDict);
				//}

				history.Add(historyModel);
			}

			//foreach (var key in masterDict.Keys)
			//{
			//	var subDict = masterDict[key];
			//	foreach (var subKey in subDict.Keys)
			//	{
			//		var list = subDict[subKey];
			//		list.OrderByDescending(x => x.ResoCount);
			//	}
			//}

			
			var trimmedList = history.GroupBy(row => row.ShiftDate)
									.SelectMany(x => x.OrderByDescending(row => row.ResoCount).Take(50))
									//.GroupBy(row => row.Rid)
									//.SelectMany(x1 => x1.OrderByDescending(row => row.ResoCount).Take(10))
									.ToList();


			// get top 100 restaurants for each day between start and endDate ordered b

			return trimmedList;
		}
	}
}