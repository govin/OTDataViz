using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OpenTableDataViz.Installers
{
	using System.Web.Mvc;

	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.Windsor;

	using OpenTableDataViz.Models;
	using OpenTableDataViz.Services;

	public class AppServicesInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
						Component.For<IAppConfiguration>()
						.ImplementedBy<AppConfigurationService>().LifeStyle.Singleton);

			container.Register(
						Component.For<IEntityOperation>()
						.ImplementedBy<EntityOpService>().LifeStyle.Singleton);

			container.Register(
						Component.For<ILogger>()
						.ImplementedBy<LoggingService>().LifeStyle.Singleton);

			container.Register(
						Component.For<IMongoDBService>()
						.ImplementedBy<MongoDBService>().LifeStyle.Singleton);

			container.Register(
						Component.For<IReservationHistoryService>()
						.ImplementedBy<ReservationHistoryService>().LifeStyle.Singleton);

		}
	}
}