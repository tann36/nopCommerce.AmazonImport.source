using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.AmazonImport
{
	public class RouteProvider : IRouteProvider
	{
		public void RegisterRoutes(RouteCollection routes)
		{
			routes.MapRoute("Plugin.Misc.AmazonImport.Search",
				"Plugins/MiscAmazonImport/SearchAmazon",
				new { controller = "MiscAmazonImport", action = "SearchAmazon" },
				new[] { "Nop.Plugin.Misc.AmazonImport.Controllers" }
		    );

			routes.MapRoute("Plugin.Misc.AmazonImport.CreateFromASIN",
				"Plugins/MiscAmazonImport/CreateFromASIN",
				new { controller = "MiscAmazonImport", action = "CreateFromASIN" },
				new[] { "Nop.Plugin.Misc.AmazonImport.Controllers" }
			);

			routes.MapRoute("Plugin.Misc.AmazonImport.AmazonImportWidget",
				"Plugins/MiscAmazonImport/AmazonImportWidget",
				new { controller = "MiscAmazonImport", action = "AmazonImportWidget" },
				new[] { "Nop.Plugin.Misc.AmazonImport.Controllers" }
			);
		}
		public int Priority
		{
			get
			{
				return 0;
			}
		}
	}
}