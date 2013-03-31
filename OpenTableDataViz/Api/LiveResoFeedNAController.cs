using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OpenTableDataViz.Api
{
	using OpenTableDataViz.Services;

	public class LiveResoFeedNAController : BaseApiController
	{
		private IBusinessQuery liveresofeed;
		
		private IAppConfiguration config;

		public LiveResoFeedNAController()
		{
		}

		public LiveResoFeedNAController(IBusinessQuery liveresofeed, IAppConfiguration config)
		{
			this.liveresofeed = liveresofeed;
			this.config = config;
		}

		public HttpResponseMessage Get()
		{
			return this.GetResponse(this.liveresofeed.GetReservationsForLastXMinutes(5, config.ResoFeedUrlNA));
		}

	}
}
