using System.ServiceProcess;


namespace OpenTableDataVizService
{
	using System.Threading;

	using Timer = System.Timers.Timer;

	public partial class OTDataVizService : ServiceBase
	{
		private ReservationFeedProcessor processor;

		private Timer timer;

		private TimerCallback timerDelegate;

		public OTDataVizService()
		{
			InitializeComponent();
			this.ServiceName = "OpenTableDataViz";
			this.CanStop = true;
			this.CanPauseAndContinue = false;
			this.AutoLog = true;
		}

		protected override void OnStart(string[] args)
		{
			processor = new ReservationFeedProcessor();
			timer = new Timer(15 * 60 * 1000) { Enabled = true };
			timer.Elapsed += timer_Elapsed;
		}

		void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			processor.Process();
		}

		protected override void OnStop()
		{
			timer.Dispose();
		}
	}
}
