using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Models
{
	using System.Configuration;

	public class AppConfigurationService : IAppConfiguration
	{
		private static string feedAPIServerBaseUrl = ConfigurationManager.AppSettings["FeedAPIServerBaseUrl"];

		public string FeedAPIServerBaseUrl
		{
			get
			{
				return feedAPIServerBaseUrl;
			}
			set
			{
				feedAPIServerBaseUrl = value;
			}
		}
	}
}