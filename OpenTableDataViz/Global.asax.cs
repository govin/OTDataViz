using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace OpenTableDataViz
{
	using Castle.Windsor;
	using Castle.Windsor.Installer;

	using OpenTableDataViz.Models;
	using OpenTableDataViz.Plumbing;
	using OpenTableDataViz.Services;

	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		private static IWindsorContainer container;

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			WebApiConfig.Register(GlobalConfiguration.Configuration);
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			AuthConfig.RegisterAuth();
			BootstrapContainer();
			CacheRestaurants();
		}

		private void CacheRestaurants()
		{
			//var entityOp = new EntityOpService(new LoggingService(), new AppConfigurationService());
			//var resoFeed = entityOp.GetEntity<ResoFeedModel>("http://feeds-na.otcorp.opentable.com/reservations/created/");
			
			var queryService = (IBusinessQuery)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IBusinessQuery));
			var restaurants = queryService.GetAllRestaurants();
			var cacheService = (ICacheService)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(ICacheService));
			cacheService.SetCacheItem(CacheKey.Restaurant, restaurants);
		}

		private static void BootstrapContainer()
		{
			container = new WindsorContainer()
				.Install(FromAssembly.This());
			GlobalConfiguration.Configuration.DependencyResolver = new WindsorDependencyResolver(container.Kernel);
			var controllerFactory = new WindsorControllerFactory(container.Kernel);
			ControllerBuilder.Current.SetControllerFactory(controllerFactory);
		}
	}
}