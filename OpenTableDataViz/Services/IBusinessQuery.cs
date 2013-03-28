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

		List<Reservation> GetReservationsForLastXMinutes(int timeToGoBackInPastMinutes, string url);

		List<ResoFeedModel> GetResoFeedsForLastXMinutes(int xminutes, string url);

		List<ResoCountBubbleChartModel> GetResoCountBubbleChartData(int timeToGoBackInPastMinutes, string url);

		List<ResoCountBubbleChartModel> GetResoCountBubbleChartData(int timeToGoBackInPastMinutes, string url, string metroArea);

		List<CuisineRadialModel> GetCuisineRadialChartData(int timeToGoBackInPastMinutes, string url);
	}
}
