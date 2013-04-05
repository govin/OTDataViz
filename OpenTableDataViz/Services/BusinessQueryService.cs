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

		private IAppConfiguration appConfig;

		private IEntityOperation entityOp;

		private ICacheService cacheService;

		private ILogger logger;

		public BusinessQueryService(IMongoDBService mongo, IAppConfiguration appConfig, IEntityOperation entityOp, ICacheService cacheService, ILogger logger)
		{
			this.mongo = mongo;
			this.appConfig = appConfig;
			this.entityOp = entityOp;
			this.cacheService = cacheService;
			this.logger = logger;
		}

		public List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate)
		{
			var collection = this.mongo.GetCollection<ReservationHistoryModel>("ReservationCount");
			var restaurants = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);


			var resoCountDict = new Dictionary<string, ReservationHistoryModel>();
			foreach (var matchingPost in collection.FindAll())
			{
				if (restaurants.ContainsKey(matchingPost.Rid))
				{
					string quarter;
					var shiftDate = DateTime.Parse(matchingPost.ShiftDate);
					if (shiftDate.Month <= 3)
					{
						quarter = shiftDate.Year + "Q1";
					}
					else if (shiftDate.Month <= 6)
					{
						quarter = shiftDate.Year + "Q2";
					}
					else if (shiftDate.Month <= 9)
					{
						quarter = shiftDate.Year + "Q3";
					}
					else
					{
						quarter = shiftDate.Year + "Q4";
					}
					var key = quarter + "-" + matchingPost.Rid;
					if (resoCountDict.ContainsKey(key))
					{
						var currentHistory = resoCountDict[key];
						currentHistory.ResoCount += matchingPost.ResoCount;
						currentHistory.TotalPartySize += matchingPost.AvgPartySize;
						currentHistory.TotalRecords += 1;
						resoCountDict[key] = currentHistory;
					}
					else
					{
						var history = new ReservationHistoryModel()
						{
							AvgPartySize = matchingPost.AvgPartySize,
							TotalRecords = 1,
							TotalPartySize = matchingPost.AvgPartySize,
							ResoCount = matchingPost.ResoCount,
							Location = restaurants[matchingPost.Rid].MetroAreaName,
							ShiftDate = quarter,
							RestaurantName = restaurants[matchingPost.Rid].RName,
							Rid = restaurants[matchingPost.Rid].RID
						};
						resoCountDict.Add(key, history);
					}
				}
			}

			var historyList = new List<ReservationHistoryModel>();
			foreach (var currentHistory in resoCountDict.Keys.Select(key => resoCountDict[key]))
			{
				currentHistory.AvgPartySize = currentHistory.TotalPartySize / currentHistory.TotalRecords;
				historyList.Add(currentHistory);
			}

			var top50List = historyList.GroupBy(row => row.ShiftDate)
									.SelectMany(x => x.OrderByDescending(row => row.ResoCount).Take(10))
				//.GroupBy(row => row.Rid)
				//.SelectMany(x1 => x1.OrderByDescending(row => row.ResoCount).Take(50))
									.ToList();


			return top50List;
		}

		public Dictionary<long, RestaurantModel> GetAllRestaurants()
		{
			var restaurantDictionary = new Dictionary<long, RestaurantModel>();
			this.ReadRestaurantsFromCsvFile(appConfig.RestaurantNaCsvFileLocation, ref restaurantDictionary);
			this.ReadRestaurantsFromCsvFile(appConfig.RestaurantEuCsvFileLocation, ref restaurantDictionary);
			this.ReadRestaurantsFromCsvFile(appConfig.RestaurantAsiaCsvFileLocation, ref restaurantDictionary);
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
			var resoList = this.GetReservationsForLastXMinutes(timeToGoBackInPastMinutes, url);
			var resoLookUpByCuisine = new Dictionary<string, List<Reservation>>();
			var restaurants = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);
			foreach (var reso in resoList)
			{
				var cuisineType = "Not Available";
				if (restaurants.ContainsKey(reso.Rid))
				{
					cuisineType = restaurants[reso.Rid].CuisineType;
				}
				if (resoLookUpByCuisine.ContainsKey(cuisineType))
				{
					var cuisineResoList = resoLookUpByCuisine[cuisineType];
					cuisineResoList.Add(reso);
				}
				else
				{
					var cuisineResoList = new List<Reservation> { reso };
					resoLookUpByCuisine.Add(cuisineType, cuisineResoList);
				}
			}

			var radialChartData = resoLookUpByCuisine.Keys.Select(cuisine => new CuisineRadialModel() { CuisineName = cuisine, ReservationCount = resoLookUpByCuisine[cuisine].Count() }).ToList();

			return radialChartData.OrderByDescending(x => x.ReservationCount).Take(3).ToList();
		}

		public List<CuisineRadialModel> GetCuisineRadialChartData(int timeToGoBackInPastMinutes, string url, string metroArea)
		{
			var resoList = this.GetReservationsForLastXMinutes(timeToGoBackInPastMinutes, url);
			resoList = resoList.Where(x => x.MetroArea == metroArea).ToList();
			var resoLookUpByCuisine = new Dictionary<string, List<Reservation>>();
			var restaurants = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);
			foreach (var reso in resoList)
			{
				var cuisineType = "Not Available";
				if (restaurants.ContainsKey(reso.Rid))
				{
					cuisineType = restaurants[reso.Rid].CuisineType;
				}
				if (resoLookUpByCuisine.ContainsKey(cuisineType))
				{
					var cuisineResoList = resoLookUpByCuisine[cuisineType];
					cuisineResoList.Add(reso);
				}
				else
				{
					var cuisineResoList = new List<Reservation> { reso };
					resoLookUpByCuisine.Add(cuisineType, cuisineResoList);
				}
			}

			var radialChartData = resoLookUpByCuisine.Keys.Select(cuisine => new CuisineRadialModel() { CuisineName = cuisine, ReservationCount = resoLookUpByCuisine[cuisine].Count() }).ToList();

			return radialChartData.OrderByDescending(x => x.ReservationCount).Take(3).ToList();
		}

		public List<Reservation> GetReservationsForLastXMinutes(int timeToGoBackInPastMinutes, string url)
		{
			var resoFeedList = this.GetResoFeedsForLastXMinutes(timeToGoBackInPastMinutes, url);

			var resoList = resoFeedList.SelectMany(resoFeedModel => resoFeedModel.Reservations).ToList();

			var restaurantDictionary = (Dictionary<long, RestaurantModel>)cacheService.GetCacheItem(CacheKey.Restaurant);

			var countByMetro = new Dictionary<string, int>();
			var countByNeighborhood = new Dictionary<string, int>();

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
					reservation.MetroArea = "Not Available";
					reservation.Neighborhood = "Not Available";
				}

				if (countByMetro.ContainsKey(reservation.MetroArea))
				{
					countByMetro[reservation.MetroArea]++;
				}
				else
				{
					countByMetro.Add(reservation.MetroArea, 1);
				}

				if (countByNeighborhood.ContainsKey(reservation.Neighborhood))
				{
					countByNeighborhood[reservation.Neighborhood]++;
				}
				else
				{
					countByNeighborhood.Add(reservation.Neighborhood, 1);
				}
			}

			// remove demoland restaurants
			resoList = resoList.Where(x => x.MetroArea != "Demoland").ToList();

			foreach (var reservation in resoList)
			{
				reservation.MetroAreaResoCount = countByMetro[reservation.MetroArea];
				reservation.NeighborhoodResoCount = countByNeighborhood[reservation.Neighborhood];
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
				var location = "Not Available";
				var cuisineType = "";
				if (restaurantDictionary.ContainsKey(rid))
				{
					var restaurant = restaurantDictionary[rid];
					location = restaurant.MetroAreaName;
					cuisineType = restaurant.CuisineType;
				}

				bubbleChartData.Add(new ResoCountBubbleChartModel()
				{
					RName = resosByRid[rid][0].RestaurantName,
					Location = location,
					CuisineType = cuisineType,
					ReservationCount = resosByRid[rid].Count,
					LocationCount = resosByRid[rid][0].MetroAreaResoCount
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
				var location = "Not Available";
				var cuisineType = "";
				if (restaurantDictionary.ContainsKey(rid))
				{
					var restaurant = restaurantDictionary[rid];
					location = restaurant.Nbhoodname;
					cuisineType = restaurant.CuisineType;
				}

				bubbleChartData.Add(new ResoCountBubbleChartModel()
				{
					RName = resosByRid[rid][0].RestaurantName,
					Location = location,
					CuisineType = cuisineType,
					ReservationCount = resosByRid[rid].Count,
					LocationCount = resosByRid[rid][0].NeighborhoodResoCount
				}
				);
			}

			return bubbleChartData.OrderByDescending(x => x.ReservationCount).Take(100).ToList();
		}

		public List<PartnerBubbleChartModel> GetPartnerBubbleChartData(int timeToGoBackInPastMinutes, string url)
		{
			var resoList = GetReservationsForLastXMinutes(timeToGoBackInPastMinutes, url);

			var resosByPartner = resoList.GroupBy(x => x.Partnername).ToDictionary(x1 => x1.Key, x1 => x1.ToList());
			var bubbleChartData = new List<PartnerBubbleChartModel>();
			foreach (var partnerName in resosByPartner.Keys)
			{
				bubbleChartData.Add(new PartnerBubbleChartModel()
				{
					PartnerName = partnerName,
					//Location = location,
					//CuisineType = cuisineType,
					ReservationCount = resosByPartner[partnerName].Count,
					//LocationCount = resosByRid[rid][0].MetroAreaResoCount
				}
				);
			}

			return bubbleChartData.OrderByDescending(x => x.ReservationCount).ToList();
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
					cacheService.SetCacheItem(feed.Href_prev, newFeed, 120);
				}
				this.GetResoFeedsForLastXMinutes(dateTimeToGoBack, newFeed, feed.Max_Resid, ref resoFeedList);
			}
		}
	}
}