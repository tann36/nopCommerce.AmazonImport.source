using FluentValidation.Attributes;
using Nop.Plugin.Misc.AmazonImport.Validators;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AmazonImport.Models
{
	public class AmazonImportModel
	{
		[Validator(typeof(CreateFromASINValidator))]
		public partial class CreateFromASINModel// : BaseNopEntityModel, ILocalizedModel<ProductVariantAttributeValueLocalizedModel> //111 localization?
		{
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.ASIN")]
			public string ASIN { get; set; }
		}

		public partial class AmazonKeywordSearchModel
		{
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.Keywords")]
			public string Keywords { get; set; }
		}

		public partial class AmazonSearchResultModel
		{
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.ProductName")]
			public string Title { get; set; }
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.ASIN")]
			public string ASIN { get; set; }
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.Description")]
			public string Description { get; set; }
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.EditorialReviews")]
			public string EditorialReviews { get; set; }
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.Price")]
			public decimal Price { get; set; }
			public string DetailPageURL { get; set; }
			[NopResourceDisplayName("Plugins.Misc.AmazonImport.Picture")]
			public string ImageUrl { get; set; }
		}
	}
}
