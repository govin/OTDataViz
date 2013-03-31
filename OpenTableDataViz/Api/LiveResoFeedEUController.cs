using System.Net.Http;

namespace OpenTableDataViz.Api
{
	using OpenTableDataViz.Services;

	public class LiveResoFeedEUController : BaseApiController
	{
		private IBusinessQuery liveresofeed;

		private IAppConfiguration config;

		public LiveResoFeedEUController()
		{
		}

		public LiveResoFeedEUController(IBusinessQuery liveresofeed, IAppConfiguration config)
		{
			this.liveresofeed = liveresofeed;
			this.config = config;
		}

		public HttpResponseMessage Get()
		{
			return this.GetResponse(this.liveresofeed.GetReservationsForLastXMinutes(5, config.ResoFeedUrlEU));
		}

	}
}
