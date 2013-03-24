using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Services
{
	public interface IAppConfiguration
	{
		string FeedAPIServerBaseUrl { get; set; }

		string ConnectionStringDataVizDB { get; set; }

		string DatabaseNameDataVizDB { get; set; }
	}
}