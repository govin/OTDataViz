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
			var restaurants = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);

			var history = new List<ReservationHistoryModel>();
			foreach (var matchingPost in collection.FindAll())
			{
				if (restaurants.ContainsKey(matchingPost.Rid))
				{
					var resoHistory = new ReservationHistoryModel()
						{
							AvgPartySize = matchingPost.AvgPartySize,
							ShiftDate = matchingPost.ShiftDate,
							Location = restaurants[matchingPost.Rid].MetroAreaName,
							ResoCount = matchingPost.ResoCount,
							RestaurantName = restaurants[matchingPost.Rid].RName,
							Rid = matchingPost.Rid
						};

					history.Add(resoHistory);
				}
			}

			var top50List = history.GroupBy(row => row.ShiftDate)
									.SelectMany(x => x.OrderByDescending(row => row.ResoCount).Take(10))
				//.GroupBy(row => row.Rid)
				//.SelectMany(x1 => x1.OrderByDescending(row => row.ResoCount).Take(10))
									.ToList();


			return top50List;
		}

		public Dictionary<long, RestaurantModel> GetAllRestaurants()
		{
			var restaurantDictionary = new Dictionary<long, RestaurantModel>();
			this.ReadRestaurantsFromCsvFile(appConfiguration.RestaurantCsvFileLocation, ref restaurantDictionary);
			this.ReadRestaurantsFromCsvFile("c:\\DataViz\\Restaurants_EU_Mar_27.csv", ref restaurantDictionary);
			this.ReadRestaurantsFromCsvFile("c:\\DataViz\\Restaurants_Asia_Mar_27.csv", ref restaurantDictionary);
			return restaurantDictionary;
		}

		private void ReadRestaurantsFromCsvFile(string csvFileLocation, ref Dictionary<long, RestaurantModel> restaurantDictionary)
		{
			try
			{
				using (var fields = new CsvReader(new StreamReader(csvFileLocation), false))
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
						//restaurant.MetroAreaID = int.Parse(fields[14]);
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
		}

		public List<CuisineRadialModel> GetCuisineRadialChartData(int timeToGoBackInPastMinutes, string url)
		{
			var resoFeedList = this.GetResoFeedsForLastXMinutes(timeToGoBackInPastMinutes, url);
			var resoLookUpByCuisine = new Dictionary<string, List<Reservation>>();
			var restaurants = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);
			foreach (var resoFeedModel in resoFeedList)
			{
				var cuisineType = "Unknown";
				if (restaurants.ContainsKey(resoFeedModel.Reservations[0].Rid))
				{
					cuisineType = restaurants[resoFeedModel.Reservations[0].Rid].CuisineType;
				}
				foreach (var reservation in resoFeedModel.Reservations)
				{
					if (resoLookUpByCuisine.ContainsKey(cuisineType))
					{
						var resoList = resoLookUpByCuisine[cuisineType];
						resoList.Add(reservation);
					}
					else
					{
						var resoList = new List<Reservation> { reservation };
						resoLookUpByCuisine.Add(cuisineType, resoList);
					}
				}
			}

			var radialChartData = resoLookUpByCuisine.Keys.Select(cuisine => new CuisineRadialModel() { CuisineName = cuisine, ReservationCount = resoLookUpByCuisine[cuisine].Count() }).ToList();

			return radialChartData.OrderByDescending(x => x.ReservationCount).Take(100).ToList();
		}

		public List<Reservation> GetReservationsForLastXMinutes(int timeToGoBackInPastMinutes, string url)
		{
			var resoFeedList = this.GetResoFeedsForLastXMinutes(timeToGoBackInPastMinutes, url);

			var resoList = resoFeedList.SelectMany(resoFeedModel => resoFeedModel.Reservations).ToList();

			var restaurantDictionary = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);

			foreach (var reservation in resoList)
			{
				var rid = reservation.Rid;
				if (restaurantDictionary.ContainsKey(rid))
				{
					var restaurant = restaurantDictionary[rid];
					reservation.MetroAreaId = restaurant.MetroAreaID;
					reservation.MetroArea = restaurant.MetroAreaName;
					reservation.Neighborhood = restaurant.Nbhoodname;
				}
				else
				{
					reservation.MetroArea = "Unknown";
					reservation.Neighborhood = "Unknown";
				}
			}

			return resoList.OrderByDescending(x => x.DateMadeUtc).ToList();
		}

		public List<ResoCountBubbleChartModel> GetResoCountBubbleChartData(int timeToGoBackInPastMinutes, string url)
		{
			var resoList = GetReservationsForLastXMinutes(timeToGoBackInPastMinutes, url);

			var restaurantDictionary = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);
			var resosByRid = resoList.GroupBy(x => x.Rid).ToDictionary(x1 => x1.Key, x1 => x1.ToList());
			var bubbleChartData = new List<ResoCountBubbleChartModel>();
			foreach (var rid in resosByRid.Keys)
			{
				var location = "Unknown";
				if (restaurantDictionary.ContainsKey(rid))
				{
					var restaurant = restaurantDictionary[rid];
					location = restaurant.MetroAreaName;
				}

				bubbleChartData.Add(new ResoCountBubbleChartModel()
				{
					RName = resosByRid[rid][0].RestaurantName,
					Location = location,
					ReservationCount = resosByRid[rid].Count
				}
				);
			}

			return bubbleChartData.OrderByDescending(x => x.ReservationCount).Take(100).ToList();
		}

		public List<ResoCountBubbleChartModel> GetResoCountBubbleChartData(int timeToGoBackInPastMinutes, string url, string metroArea)
		{
			var resoList = this.GetReservationsForLastXMinutes(timeToGoBackInPastMinutes, url);

			resoList = resoList.Where(x => x.MetroArea == metroArea).ToList();

			var restaurantDictionary = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);
			var resosByRid = resoList.GroupBy(x => x.Rid).ToDictionary(x1 => x1.Key, x1 => x1.ToList());
			var bubbleChartData = new List<ResoCountBubbleChartModel>();
			foreach (var rid in resosByRid.Keys)
			{
				var location = "Unknown";
				if (restaurantDictionary.ContainsKey(rid))
				{
					var restaurant = restaurantDictionary[rid];
					location = restaurant.Nbhoodname;
				}

				bubbleChartData.Add(new ResoCountBubbleChartModel()
				{
					RName = resosByRid[rid][0].RestaurantName,
					Location = location,
					ReservationCount = resosByRid[rid].Count
				}
				);
			}

			return bubbleChartData.OrderByDescending(x => x.ReservationCount).Take(100).ToList();
		}

		public List<ResoFeedModel> GetResoFeedsForLastXMinutes(int xminutes, string url)
		{
			var timeToGoBack = DateTime.UtcNow.AddMinutes(xminutes * -1);
			var feed = entityOp.GetEntity<ResoFeedModel>(url);
			var resoFeedList = new List<ResoFeedModel>();
			this.GetResoFeedsForLastXMinutes(timeToGoBack, feed, -1, ref resoFeedList);
			return resoFeedList;
		}

		private void GetResoFeedsForLastXMinutes(DateTime dateTimeToGoBack, ResoFeedModel feed, long prevMaxResId, ref List<ResoFeedModel> resoFeedList)
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
				this.GetResoFeedsForLastXMinutes(dateTimeToGoBack, newFeed, feed.Max_Resid, ref resoFeedList);
			}
		}
	}
}