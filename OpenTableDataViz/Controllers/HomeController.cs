using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenTableDataViz.Controllers
{
	using OpenTableDataViz.Models;

	public class HomeController : Controller
	{
		private IAppConfiguration config;
		public HomeController(IAppConfiguration config)
		{
			this.config = config;
		}
		public ActionResult Index()
		{
			ViewBag.Message = "Modify this template to jump-start your ASP.NET MVC application.";

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your app description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}
