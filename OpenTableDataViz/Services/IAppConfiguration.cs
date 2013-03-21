using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Models
{
	public interface IAppConfiguration
	{
		string FeedAPIServerBaseUrl { get; set; }
	}
}