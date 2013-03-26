using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OpenTableDataViz.Api
{
	using OpenTableDataViz.Services;

	public class ReservationHistoryController : BaseApiController
	{
		private IReservationHistoryService history;

		public ReservationHistoryController()
		{
			
		}

		public ReservationHistoryController(IReservationHistoryService history)
		{
			this.history = history;
		}

        public HttpResponseMessage Get()
        {
	        var localhistory = new ReservationHistoryService(new MongoDBService(new AppConfigurationService(), new LoggingService()), 
				new AppConfigurationService());
			return this.GetResponse(localhistory.GetHistory("blah", "blah"));
        }
    }
}
