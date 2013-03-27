using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenTableDataViz.Api
{
	using System.Net.Http;
	using System.Net.Http.Formatting;
	using System.Web.Http;

	using Newtonsoft.Json;

	public class BaseApiController : ApiController
	{
		protected HttpResponseMessage GetResponse<T>(T data)
		{
			var response = new HttpResponseMessage();
			var formatter = new JsonMediaTypeFormatter()
				{
					SerializerSettings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Include }
				};
			response.Content = new ObjectContent<T>(data, formatter);
			return response;
		}
	}
}
