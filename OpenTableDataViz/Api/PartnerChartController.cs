using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OpenTableDataViz.Api
{
	using OpenTableDataViz.Services;

	public class PartnerChartController : BaseApiController
	{
		private readonly IBusinessQuery query;

		private readonly IAppConfiguration config;

		public PartnerChartController()
		{
		}

		public PartnerChartController(IBusinessQuery query, IAppConfiguration config)
		{
			this.query = query;
			this.config = config;
		}

		[HttpGet]
		public HttpResponseMessage Get(string region)
		{
			switch (region.ToLower().Trim())
			{
				case "eu":
					return this.GetResponse(this.query.GetPartnerBubbleChartData(60, this.config.ResoFeedUrlEU));
				case "asia":
					return this.GetResponse(this.query.GetPartnerBubbleChartData(60, this.config.ResoFeedUrlAsia));
				default:
					return this.GetResponse(this.query.GetPartnerBubbleChartData(60, config.ResoFeedUrlNA));
			}
		}
	}
}
