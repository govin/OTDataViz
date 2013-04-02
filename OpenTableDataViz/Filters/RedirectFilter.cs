using System.Linq;
using System.Web.Routing;


namespace OpenTableDataViz.Filters
{
	using System.Web.Mvc;

	public class RedirectFilter : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext filterContext)
		{
			var request = filterContext.HttpContext.Request;

			var urlList = new string[] { "/na", "/eu", "/jp" };

			if (request.Url != null)
			{
				var absPath = request.Url.AbsolutePath.ToLower().TrimEnd('/');
				var index = absPath.LastIndexOf("/");
				var lastPartOfUrl = string.Empty;
				if (index > -1)
				{
					lastPartOfUrl = absPath.Substring(index).ToLower();
				}

				if (urlList.Contains(lastPartOfUrl))
				{

					filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary()
							{
								{ "action", "Live" },
								{ "controller", "Map" },
							});
				}
			}
		}
	}
}