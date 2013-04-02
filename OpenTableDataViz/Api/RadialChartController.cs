using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Api
{
    using System.Net.Http;
    using System.Web.Http;

    using OpenTableDataViz.Services;

    public class RadialChartController : BaseApiController
    {
        private IBusinessQuery query;

        private IAppConfiguration config;

        public RadialChartController()
        {
        }

        public RadialChartController(IBusinessQuery query, IAppConfiguration configuration)
        {
            this.query = query;
            this.config = configuration;
        }

        [HttpGet]
        public HttpResponseMessage Get(string region)
        {
            switch (region.ToLower().Trim())
            {
                case "eu":
                    return this.GetResponse(this.query.GetCuisineRadialChartData(60, this.config.ResoFeedUrlEU));
                case "asia":
                    return this.GetResponse(this.query.GetCuisineRadialChartData(60, this.config.ResoFeedUrlAsia));
                default:
                    return this.GetResponse(this.query.GetCuisineRadialChartData(60, config.ResoFeedUrlNA));
            }
        }

        public HttpResponseMessage Get(string region, string metro)
        {
            switch (region.ToLower().Trim())
            {
                case "eu":
                    return this.GetResponse(this.query.GetCuisineRadialChartData(60, this.config.ResoFeedUrlEU, metro));
                case "asia":
                    return this.GetResponse(this.query.GetCuisineRadialChartData(60, this.config.ResoFeedUrlAsia, metro));
                default:
                    return this.GetResponse(this.query.GetCuisineRadialChartData(60, config.ResoFeedUrlNA, metro));
            }
        }
    }
}