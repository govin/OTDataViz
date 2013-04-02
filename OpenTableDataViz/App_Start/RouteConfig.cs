using System.Web.Mvc;
using System.Web.Routing;

namespace OpenTableDataViz
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

			routes.MapRoute(
				"NARoute",
				"na",
				new { Controller = "Map", Action = "Live", id = UrlParameter.Optional });

			routes.MapRoute(
				"EURoute",
				"eu",
				new { Controller = "Map", Action = "Live", id = UrlParameter.Optional });

			routes.MapRoute(
				"AsiaRoute",
				"jp",
				new { Controller = "Map", Action = "Live", id = UrlParameter.Optional });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Map", action = "Live", id = UrlParameter.Optional }
			);

			
		}
	}
}