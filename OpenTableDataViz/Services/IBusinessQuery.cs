using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTableDataViz.Services
{
	using OpenTableDataViz.Models;

	public interface IBusinessQuery
	{
		List<ReservationHistoryModel> GetHistory(string isoStartDate, string isoEndDate);

		Dictionary<long, RestaurantModel> GetAllRestaurants();

		List<ResoFeedModel> GetReservationsMadeInTheLastXMinutes(int xminutes);

		List<ResoCountBubbleChartModel> GetResoCountBubbleChartData(int timeToGoBackInPastMinutes);

		List<CuisineRadialModel> GetCuisineRadialChartData(int timeToGoBackInPastMinutes);
	}
}
