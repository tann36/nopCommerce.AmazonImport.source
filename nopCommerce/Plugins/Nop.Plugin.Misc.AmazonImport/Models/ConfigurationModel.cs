using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Misc.AmazonImport.Models
{
	public class ConfigurationModel : BaseNopModel
	{
		[NopResourceDisplayName("Plugins.Misc.AmazonImport.UpdateIntervalMinutes")]
		public double UpdateInterval { get; set; }
		[NopResourceDisplayName("Plugins.Misc.AmazonImport.UpdateAutomatically")]
		public bool UpdatingEnabled { get; set; }
	}
}
