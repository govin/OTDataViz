using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Api
{
	using System.Net.Http;

	using OpenTableDataViz.Services;

	public class RadialChartController : BaseApiController
	{
		private IBusinessQuery query;

		public RadialChartController()
		{
		}

		public RadialChartController(IBusinessQuery query)
		{
			this.query = query;
		}

		public HttpResponseMessage Get()
		{
			return this.GetResponse(this.query.GetCuisineRadialChartData(24));
		}
	}
}