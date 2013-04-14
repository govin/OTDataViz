using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OpenTableDataViz.Services;
using System.Net.Http;

namespace OpenTableDataViz.Api
{
	public class ReservationController : BaseApiController
	{
		private IBusinessQuery liveresofeed;

		private IAppConfiguration config;

		public ReservationController()
		{
		}

		public ReservationController(IBusinessQuery liveresofeed, IAppConfiguration config)
		{
			this.liveresofeed = liveresofeed;
			this.config = config;
		}

		public HttpResponseMessage Get(string rids, int? interval)
		{
            int timeInterval;
            if (interval.HasValue)
            {
                if (interval == 0 || interval > 36 * 60)
                {
                    throw new ApplicationException("Invalid interval specified");
                }

                timeInterval = (int) interval;
            }
            else
            {
                timeInterval = 60;
            }

            var restList = rids.Split(',');
            var ridlist = new List<long>();
            foreach (var restaurant in restList)
            {
                ridlist.Add(long.Parse(restaurant));
            }
            var resolist = this.liveresofeed.GetReservationsForLastXMinutes(timeInterval, config.ResoFeedUrlNA);
            resolist = resolist.Where(x => ridlist.Contains(x.Rid)).ToList();
			return this.GetResponse(resolist);
		}

	}
}
