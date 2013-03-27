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
		private IBusinessQuery history;

		public ReservationHistoryController()
		{	
		}

		public ReservationHistoryController(IBusinessQuery history)
		{
			this.history = history;
		}

        public HttpResponseMessage Get()
        {   
			return this.GetResponse(this.history.GetHistory("blah", "blah"));
        }
    }
}
