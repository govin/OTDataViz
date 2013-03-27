namespace OpenTableDataViz.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	using LumenWorks.Framework.IO.Csv;

	using MongoDB.Driver;

	using OpenTableDataViz.Models;

	using System.Linq;

	public class BusinessQueryService : IBusinessQuery
	{
		private IMongoDBService mongo;

		private IAppConfiguration appConfiguration;

		private IEntityOperation entityOp;

		private ICacheService cacheService;

		private ILogger logger;

		public BusinessQueryService(IMongoDBService mongo, IAppConfiguration appConfiguration, IEntityOperation entityOp, ICacheService cacheService, ILogger logger)
		{
			this.mongo = mongo;
			this.appConfiguration = appConfiguration;
			this.entityOp = entityOp;
			this.cacheService = cacheService;
			this.logger = logger;
		}

		public List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate)
		{
			var collection = this.mongo.GetCollection<ReservationHistoryModel>("ReservationCount");
			var history = collection.FindAll().Select(matchingPost => 
				new ReservationHistoryModel() 
				{ 
					AvgPartySize = matchingPost.AvgPartySize, ShiftDate = matchingPost.ShiftDate, 
					//Location = "San Francisco", 
					ResoCount = matchingPost.ResoCount,
					//RestaurantName = matchingPost.RestaurantName,
					Rid = matchingPost.Rid 
				}
				).ToList();

			var top50List = history.GroupBy(row => row.ShiftDate)
									.SelectMany(x => x.OrderByDescending(row => row.ResoCount).Take(50))
									//.GroupBy(row => row.Rid)
									//.SelectMany(x1 => x1.OrderByDescending(row => row.ResoCount).Take(10))
									.ToList();


			return top50List;
		}

		public Dictionary<long, RestaurantModel> GetAllRestaurants()
		{
			//var collection = this.mongo.GetCollection<RestaurantModel>("Restaurant");
			////return collection.FindAll().ToList();
			//var restaurantList = collection.FindAll().SetFields("RID", "RName", "Nbhoodname", "MetroAreaName").Select(restaurant => new RestaurantModel()
			//	{
			//		RID = restaurant.RID,
			//		RName = restaurant.RName,
			//		Nbhoodname = restaurant.Nbhoodname,
			//		MetroAreaName = restaurant.MetroAreaName
			//	}).ToList();
			//return restaurantList;
			
			var restaurantDictionary = new Dictionary<long, RestaurantModel>();
			try
			{
				using (var fields = new CsvReader(new StreamReader(appConfiguration.RestaurantCsvFileLocation), false))
				{	
					while (fields.ReadNextRecord())
					{
						var restaurant = new RestaurantModel();
						//var fields = line.Split(',');
						var rid = long.Parse(fields[0]);
						restaurant.RID = rid;
						restaurant.RName = fields[1];
						restaurant.RestaurantType = fields[2];
						//restaurant.RestStateID = int.Parse(fields[3]);
						restaurant.ExternalURL = fields[4];
						//restaurant.Latitude = double.Parse(fields[5]);
						//restaurant.Longitude = double.Parse(fields[6]);
						restaurant.MenuURL = fields[7];
						//restaurant.LanguageID = int.Parse(fields[8]);
						restaurant.FacebookURL = fields[9];
						// restaurant.ZIP = fields[10];
						restaurant.Country = fields[11];
						//restaurant.NeighborhoodID = int.Parse(fields[12]);
						restaurant.Nbhoodname = fields[13];
						//restaurant.MetroAreaID = int.TryParse(fields[14], out restaurant.MetroAreaID);
						restaurant.MetroAreaName = fields[15];
						restaurant.CuisineType = fields[16];
						restaurant.DiningStyle = fields[17];

						if (restaurantDictionary.ContainsKey(rid))
						{
							//throw new Exception("Two restaurants with same rid found");
						}
						else
						{
							restaurantDictionary.Add(rid, restaurant);
						}
					}
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex);
			}

			return restaurantDictionary;
		}

		public List<CuisineRadialModel> GetCuisineRadialChartData(int timeToGoBackInPastMinutes)
		{
			var resoFeedList = GetReservationsMadeInTheLastXMinutes(timeToGoBackInPastMinutes);
			var resoLookUpByCuisine = new Dictionary<string, List<Reservation>>();
			var restaurants = (Dictionary<long, RestaurantModel>) cacheService.GetCacheItem(CacheKey.Restaurant);
			foreach (var resoFeedModel in resoFeedList)
			{
				var restaurant = restaurants[resoFeedModel.Reservations[0].Rid];
				foreach (var reservation in resoFeedModel.Reservations)
				{
					if (resoLookUpByCuisine.ContainsKey(restaurant.CuisineType))
					{
						var resoList = resoLookUpByCuisine[restaurant.CuisineType];
						resoList.Add(reservation);
					}
					else
					{
						var resoList = new List<Reservation> { reservation };
						resoLookUpByCuisine.Add(restaurant.CuisineType, resoList);
					}
				}
			}

			var radialChartData = resoLookUpByCuisine.Keys.Select(cuisine => new CuisineRadialModel() { CuisineName = cuisine, ReservationCount = resoLookUpByCuisine[cuisine].Count() }).ToList();

			return radialChartData.OrderByDescending(x => x.ReservationCount).Take(100).ToList();
		}

		public List<ResoCountBubbleChartModel> GetResoCountBubbleChartData(int timeToGoBackInPastMinutes)
		{
			var resoFeedList = GetReservationsMadeInTheLastXMinutes(timeToGoBackInPastMinutes);
			var resoLookUpByRid = new Dictionary<long, List<Reservation>>();
			foreach (var resoFeedModel in resoFeedList)
			{
				foreach (var reservation in resoFeedModel.Reservations)
				{
					if (resoLookUpByRid.ContainsKey(reservation.Rid))
					{
						var resoList = resoLookUpByRid[reservation.Rid];
						resoList.Add(reservation);
					}
					else
					{
						var resoList = new List<Reservation> { reservation };
						resoLookUpByRid.Add(reservation.Rid, resoList);
					}
				}
			}

			var restaurantDictionary = (Dictionary<long, RestaurantModel>) cacheService.GetCacheItem(CacheKey.Restaurant);
			var bubbleChartData = new List<ResoCountBubbleChartModel>();
			foreach (var rid in resoLookUpByRid.Keys)
			{
				var location = "Unknown";
				if (restaurantDictionary.ContainsKey(rid))
				{
					var restaurant = restaurantDictionary[rid];
					location = restaurant.MetroAreaName;
				}

				bubbleChartData.Add(new ResoCountBubbleChartModel()
					{
						RName = resoLookUpByRid[rid][0].RestaurantName,
						Location = location,
						ReservationCount = resoLookUpByRid[rid].Count
					}
				);
			}

			return bubbleChartData.OrderByDescending(x => x.ReservationCount).Take(100).ToList();
		}

		public List<ResoFeedModel> GetReservationsMadeInTheLastXMinutes(int xminutes)
		{
			var timeToGoBack = DateTime.UtcNow.AddMinutes(xminutes * -1);
			var startUrl = appConfiguration.ResoFeedUrlNA;
			var feed = entityOp.GetEntity<ResoFeedModel>(startUrl);
			var resoFeedList = new List<ResoFeedModel>();
			this.GetReservations(timeToGoBack, feed, -1, ref resoFeedList);
			return resoFeedList;
		}

		private void GetReservations(DateTime dateTimeToGoBack, ResoFeedModel feed, long prevMaxResId, ref List<ResoFeedModel> resoFeedList)
		{
			if (dateTimeToGoBack < feed.Max_DateMadeUtc && feed.Max_Resid != prevMaxResId)
			{
				resoFeedList.Add(feed);
				var cacheItem = cacheService.GetCacheItem(feed.Href_prev);
				ResoFeedModel newFeed;
				if (cacheItem != null)
				{
					newFeed = (ResoFeedModel)cacheItem;
				}
				else
				{
					newFeed = entityOp.GetEntity<ResoFeedModel>(feed.Href_prev);	
					cacheService.SetCacheItem(feed.Href_prev, newFeed);
				}
				this.GetReservations(dateTimeToGoBack, newFeed, feed.Max_Resid, ref resoFeedList);
			}
		}

		//public List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate)
		//{
		//	var masterDict = new Dictionary<string, Dictionary<long, List<ReservationHistoryModel>>>();
		//	var history = new List<ReservationHistoryModel>();

		//	var collection1 = this.mongo.GetCollection<ReservationHistoryModel>("ReservationCount");
		//	foreach (var matchingPost in collection1.FindAll())
		//	{
		//		var historyModel = new ReservationHistoryModel()
		//		{
		//			AvgPartySize = matchingPost.AvgPartySize,
		//			ShiftDate = matchingPost.ShiftDate,
		//			//Location = "San Francisco", 
		//			ResoCount = matchingPost.ResoCount,
		//			//RestaurantName = matchingPost.RestaurantName,
		//			Rid = matchingPost.Rid
		//		};
		//		//var isoShiftDate = matchingPost.ShiftDate;
		//		//var rid = matchingPost.Rid;
		//		//if (masterDict.ContainsKey(isoShiftDate))
		//		//{
		//		//	var subDict = masterDict[isoShiftDate];
		//		//	if (subDict.ContainsKey(rid))
		//		//	{
		//		//		var list = subDict[rid];
		//		//		list.Add(historyModel);
		//		//	}
		//		//	else
		//		//	{
		//		//		var list = new List<ReservationHistoryModel> { historyModel };
		//		//		subDict.Add(rid, list);
		//		//	}
		//		//}
		//		//else
		//		//{
		//		//	var subDict = new Dictionary<long, List<ReservationHistoryModel>>();
		//		//	var list = new List<ReservationHistoryModel> { historyModel };
		//		//	subDict.Add(rid, list);
		//		//	masterDict.Add(isoShiftDate, subDict);
		//		//}

		//		history.Add(historyModel);
		//	}

		//	//foreach (var key in masterDict.Keys)
		//	//{
		//	//	var subDict = masterDict[key];
		//	//	foreach (var subKey in subDict.Keys)
		//	//	{
		//	//		var list = subDict[subKey];
		//	//		list.OrderByDescending(x => x.ResoCount);
		//	//	}
		//	//}


		//	var trimmedList = history.GroupBy(row => row.ShiftDate)
		//							.SelectMany(x => x.OrderByDescending(row => row.ResoCount).Take(50))
		//		//.GroupBy(row => row.Rid)
		//		//.SelectMany(x1 => x1.OrderByDescending(row => row.ResoCount).Take(10))
		//							.ToList();


		//	// get top 100 restaurants for each day between start and endDate ordered b

		//	return trimmedList;
		//}
	}
}