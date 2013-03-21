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

	public class AppServicesInstaller : IWindsorInstaller
	{
		public void Install(IWindsorContainer container, IConfigurationStore store)
		{
			container.Register(
						Component.For<IAppConfiguration>()
						.ImplementedBy<AppConfigurationService>());

		}
	}
}