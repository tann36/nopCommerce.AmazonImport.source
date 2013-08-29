using Nop.Plugin.Misc.AmazonImport.AmazonApiServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Misc.AmazonImport.AmazonHelper
{
	public class AmazonItem
	{
		public string ASIN { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Reviews { get; set; }
		public decimal Price { get; set; }
		public string DetailPageURL { get; set; }
		public string ImageUrl { get; set; }

		
		AmazonItem()
		{
		}

		public AmazonItem(Item item)
		{
			if (item == null) return;

			Title = item.ItemAttributes.Title;
			ASIN = item.ASIN;

			if (item.EditorialReviews != null)
			{
				Description = item.EditorialReviews[0].Content;

				if (item.EditorialReviews.Length > 1)
					Reviews = item.EditorialReviews[1].Content;
			}

			DetailPageURL = item.DetailPageURL;

			if (item.OfferSummary != null)
			{
				Price = 0;
				if (item.OfferSummary.LowestNewPrice != null)//there are no unused offers
					if(item.OfferSummary.LowestNewPrice.Amount != null)//offer is too low to display
						Price = Decimal.Parse(item.OfferSummary.LowestNewPrice.Amount) / 100;
			}

			if (item.SmallImage != null)
				ImageUrl = item.LargeImage.URL;

			return;
		}
	}
}