using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Plugin.Misc.AmazonImport
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class AmazonImportPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;

        public AmazonImportPlugin(
			IPictureService pictureService, 
            ISettingService settingService)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
			return new List<string>() 
			{
				/*"admin_header_after",
				"admin_header_before",
				"admin_header_middle",
				"admin_dashboard_top",*/
				"admin_dashboard_bottom"
			};
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "MiscAmazonImport";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Misc.AmazonImport.Controllers" }, { "area", null } };
        }

		/// <summary>
		/// Gets a route for displaying widget
		/// </summary>
		/// <param name="widgetZone">Widget zone where it's displayed</param>
		/// <param name="actionName">Action name</param>
		/// <param name="controllerName">Controller name</param>
		/// <param name="routeValues">Route values</param>
		public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
		{
			actionName = "AmazonImportWidget";
			controllerName = "MiscAmazonImport";
			routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Misc.AmazonImport.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
		}
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
		{
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.SearchAmazon", "Search on Amazon");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.CreateFromASIN", "Create from ASIN");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.Create", "Create");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.ASIN", "ASIN");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.Keywords", "Keywords");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.ProductName", "Product name");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.Description", "Description");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.EditorialReviews", "Editorial reviews");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.Price", "Price");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.Picture", "Picture");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.UpdateIntervalMinutes", "Update interval in minutes");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.UpdateAutomatically", "Update automatically");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.ASINRequired", "An ASIN is required");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.Search", "Search");
			this.AddOrUpdatePluginLocaleResource("Plugins.Misc.AmazonImport.AddToStore", "Add to store");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
		{
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.AmazonSearch");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.CreateFromASIN");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.Create");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.ASIN");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.Keywords");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.ProductName");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.Description");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.EditorialReviews");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.Price");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.Picture");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.UpdateIntervalMinutes");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.UpdateAutomatically");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.ASINRequired");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.Search");
			this.DeletePluginLocaleResource("Plugins.Misc.AmazonImport.AddToStore");
            base.Uninstall();
        }
    }
}
