using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Api
{
	using System.Net.Http;

	using OpenTableDataViz.Services;

	public class BubbleChartNAController : BaseApiController
	{
		private IBusinessQuery query;

		public BubbleChartNAController()
		{	
		}

		public BubbleChartNAController(IBusinessQuery query)
		{
			this.query = query;
		}

        public HttpResponseMessage Get()
        {   
			return this.GetResponse(this.query.GetResoCountBubbleChartData(24));
        }
	}
}