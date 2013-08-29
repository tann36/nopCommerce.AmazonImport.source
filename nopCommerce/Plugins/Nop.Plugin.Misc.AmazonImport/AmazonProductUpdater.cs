using Nop.Core.Domain.Catalog;
using Nop.Plugin.Misc.AmazonImport.AmazonHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.AmazonImport
{
    public class AmazonProductUpdater
    {
		private Nop.Services.Catalog.IProductService _productService;
		private System.Timers.Timer updateTimer;
		private const int INITIAL_INTERVAL = 30 * 60 * 1000;

		/// <summary>
		/// Gets or sets the time interval in minutes
		/// </summary>
		public double UpdateInterval
		{
			get { return updateTimer.Interval / 60 / 1000;}
			set { updateTimer.Interval = value * 60 * 1000; }
		}

		public bool Enabled
		{
			get { return updateTimer.Enabled; }
			set { updateTimer.Enabled = value; }
		}

		public AmazonProductUpdater(Nop.Services.Catalog.IProductService productService)
		{
			//productService for accessing products on the database
			_productService = productService;

			//create a timer to update imported item prices
			updateTimer = new System.Timers.Timer(INITIAL_INTERVAL);
			updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
		}

		//updates the prices of imported products on the database
		private void OnTimedEvent(object source, System.Timers.ElapsedEventArgs e)
		{
			System.Diagnostics.Debug.WriteLine("--- updating amazon items --- ");//333

			//get all products from the database
			var allProducts = _productService.SearchProducts(showHidden: true);

			var products = new List<Product>();
			var ASINs = new List<string>();
			Regex r = new Regex(@"ASIN=(\w+);");
			
			//add all amazon products and their ASINs to Lists
			foreach (var product in allProducts)
			{
				if (product.AdminComment != null)
				{
					Match m = r.Match(product.AdminComment);
					if (m.Success)
					{
						products.Add(product);
						ASINs.Add(m.Groups[1].ToString());
					}
				}
			}
			
			//use ASINs to get information on all of the products using the Amazon API
			AmazonItem[] items = AmazonSearcher.GetItems(ASINs.ToArray());

			//if item prices have changed, update them on the database
			for (int i = 0; i < products.Count; i++)
			{
				if (items[i] == null) // caused if item ASIN was not found or there was a duplicate
				{
					//System.Diagnostics.Debug.WriteLine("problem occured when updating " + products[i].Name);
					products[i].Published = false;
					products[i].UpdatedOnUtc = DateTime.UtcNow;
					_productService.UpdateProduct(products[i]);
				}
				else if (items[i].Price == 0) // there were no unused items or the price was unavailable
				{
					//System.Diagnostics.Debug.WriteLine("price for item " + items[i].Title + " was null");
					//products[i].Price = 0;
					products[i].Published = false;
					products[i].UpdatedOnUtc = DateTime.UtcNow;
					_productService.UpdateProduct(products[i]);
				}
				else if (products[i].Price != items[i].Price) // item was found, update price if different
				{
					products[i].Price = items[i].Price;
					products[i].UpdatedOnUtc = DateTime.UtcNow;
					_productService.UpdateProduct(products[i]);
				}
			}//end for
		}

		private IList<Product> GetAllImportedProducts()
		{
			var products = new List<Product>();

			Regex r = new Regex(@"ASIN=(\w+);");
			var allProducts = _productService.SearchProducts();
			foreach (var product in allProducts)
			{
				Match m = r.Match(product.AdminComment);
				if (m.Success) products.Add(product);
			}
			return products;
		}
    }
}
