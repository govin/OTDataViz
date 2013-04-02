using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Services
{
    public interface IAppConfiguration
    {
        string ResoFeedUrlNA { get; set; }

        string ResoFeedUrlEU { get; set; }

        string ResoFeedUrlAsia { get; set; }

        string ConnectionStringDataVizDB { get; set; }

        string DatabaseNameDataVizDB { get; set; }

        string RestaurantNaCsvFileLocation { get; set; }

        string RestaurantEuCsvFileLocation { get; set; }

        string RestaurantAsiaCsvFileLocation { get; set; }
    }
}