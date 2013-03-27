using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Services
{
	using System.Configuration;

	public class AppConfigurationService : IAppConfiguration
	{
		private static string feedAPIServerBaseUrl = ConfigurationManager.AppSettings["FeedAPIServerBaseUrl"];

		private static string dataVizDbConnectionString = ConfigurationManager.ConnectionStrings["DataVizDB"].ConnectionString;

		private static string dvizDatabaseName = ConfigurationManager.AppSettings["DatabaseNameDataVizDB"];

		private static string resoFeedUrlNA = ConfigurationManager.AppSettings["ResoFeedUrlNA"];

		private static string resoFeedUrlEU = ConfigurationManager.AppSettings["ResoFeedUrlEU"];

		private static string resoFeedUrlAsia = ConfigurationManager.AppSettings["ResoFeedUrlAsia"];

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

		public string ConnectionStringDataVizDB
		{
			get
			{
				return dataVizDbConnectionString;
			}
			set
			{
				dataVizDbConnectionString = value;
			}
		}

		public string DatabaseNameDataVizDB
		{
			get
			{
				return dvizDatabaseName;
			}
			set
			{
				dvizDatabaseName = value;
			}
		}

		public string ResoFeedUrlNA
		{
			get
			{
				return resoFeedUrlNA;
			}
			set
			{
				resoFeedUrlNA = value;
			}
		}

		public string ResoFeedUrlEU
		{
			get
			{
				return resoFeedUrlEU;
			}
			set
			{
				resoFeedUrlEU = value;
			}
		}

		public string ResoFeedUrlAsia
		{
			get
			{
				return resoFeedUrlAsia;
			}
			set
			{
				resoFeedUrlAsia = value;
			}
		}
	}
}