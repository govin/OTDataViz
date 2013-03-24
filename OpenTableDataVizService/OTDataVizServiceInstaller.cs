using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace OpenTableDataVizService
{
	using System.ServiceProcess;

	[RunInstaller(true)]
	public partial class OTDataVizServiceInstaller : System.Configuration.Install.Installer
	{
		private ServiceProcessInstaller processInstaller;

		private ServiceInstaller serviceInstaller;

		public OTDataVizServiceInstaller()
		{
			InitializeComponent();

			processInstaller = new ServiceProcessInstaller();
			serviceInstaller = new ServiceInstaller();

			processInstaller.Account = ServiceAccount.LocalSystem;
			serviceInstaller.StartType = ServiceStartMode.Manual;
			serviceInstaller.ServiceName = "OpenTableDataViz";

			Installers.Add(serviceInstaller);
			Installers.Add(processInstaller);
		}
	}
}
