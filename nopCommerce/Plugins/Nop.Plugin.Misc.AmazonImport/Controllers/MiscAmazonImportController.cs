using Nop.Plugin.Misc.AmazonImport.Models;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Security;
using System;
using System.Linq;
using System.Net;
using System.Web;
using Telerik.Web.Mvc;
using System.Web.Mvc;
using System.Web.Routing;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nop.Plugin.Misc.AmazonImport.AmazonHelper;
using Nop.Core.Data;
using Nop.Services.Logging;

namespace Nop.Plugin.Misc.AmazonImport.Controllers
{
	public class MiscAmazonImportController : Controller
	{
		private readonly IProductService _productService;
        private readonly IPictureService _pictureService;
		private readonly IPermissionService _permissionService;
		private readonly ILogger _logger;
		private readonly IRepository<ProductTemplate> _productTemplateRepository;
		private static AmazonProductUpdater _amazonProductUpdater;


		public MiscAmazonImportController(
			IProductService productService, 
			IPictureService pictureService,
            IPermissionService permissionService,
			ILogger logger,
			IRepository<ProductTemplate> productTemplateRepository)
		{
			//System.Diagnostics.Debug.WriteLine("--- amazon import controller --- \n"); //333
			if(_amazonProductUpdater == null)
				_amazonProductUpdater = new AmazonProductUpdater(productService);

			_productService = productService;
			_pictureService = pictureService;
			_permissionService = permissionService;
			_logger = logger;
			_productTemplateRepository = productTemplateRepository;
		}

		/// <summary>
		/// Gets widget zones where this widget should be rendered
		/// </summary>
		/// <returns>Widget zones</returns>
		public IList<string> GetWidgetZones()
		{
			return new List<string>() { "admin_dashboard_bottom" };
		}

		private IList<string> GetAllAmazonProductASINs()
		{
			var ASINs = new List<string>();

			Regex r = new Regex(@"ASIN=(\w+);");
			var allProducts = _productService.SearchProducts(showHidden:true);
			foreach (var product in allProducts)
			{
				if(product.AdminComment != null)
				{
					Match m = r.Match(product.AdminComment);

					if (m.Success) ASINs.Add(m.Groups[1].ToString());//ASINs.Add(m.Groups[1].ToString());
				}
			}
			return ASINs;
		}

		//add an AmazonItem to the database and return its id
		private int AddAmazonItem(AmazonItem item)
		{
			//create a Product object from the model
			var product = new Product()
			{
				ProductType = ProductType.SimpleProduct, 
                VisibleIndividually = true,
                Name = item.Title,
				AdminComment = "ASIN=" + item.ASIN + ";",
				FullDescription = item.Description,

				//the following properties were copy-pasted from samples
				Price = item.Price,
				ProductTemplateId = _productTemplateRepository.Table.FirstOrDefault(pt => pt.Name == "Simple product").Id,
                AllowCustomerReviews = true,
                OrderMinimumQuantity = 1,
                OrderMaximumQuantity = 10000,
				ManageInventoryMethod = ManageInventoryMethod.ManageStock,
				NotifyAdminForQuantityBelow = 1,
				DisplayStockAvailability = true,
				LowStockActivity = LowStockActivity.DisableBuyButton,
				BackorderMode = BackorderMode.NoBackorders,
                AllowBackInStockSubscriptions = false,
                Published = true,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
			};
			
			//add an image to the product
			byte[] imageBinary;
			using (var client = new WebClient())
			{
				imageBinary = client.DownloadData(item.ImageUrl);
			}
			product.ProductPictures.Add(new ProductPicture()
			{
				Picture = _pictureService.InsertPicture(imageBinary, MimeMapping.GetMimeMapping(item.ImageUrl), _pictureService.GetPictureSeName(product.Name), true),
				DisplayOrder = 1,
			});

			//add the product to the database
			_productService.InsertProduct(product);

			return product.Id;
		}


		public ActionResult Configure()
		{
			var model = new ConfigurationModel();
			model.UpdatingEnabled = _amazonProductUpdater.Enabled;
			model.UpdateInterval = _amazonProductUpdater.UpdateInterval;

			return View("Nop.Plugin.Misc.AmazonImport.Views.MiscAmazonImport.Configure", model);
		}

		[HttpPost]
		public ActionResult Configure(ConfigurationModel model)
		{
			if (ModelState.IsValid)
			{
				_amazonProductUpdater.Enabled = model.UpdatingEnabled;
				_amazonProductUpdater.UpdateInterval = model.UpdateInterval;
			} //if (ModelState.IsValid)

			return Configure();
		}

		public ActionResult AmazonImportWidget()
		{
			//allow this page to been seen only after logging in
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return null;

			return View("Nop.Plugin.Misc.AmazonImport.Views.MiscAmazonImport.AmazonImportWidget");
		}

		public ActionResult SearchAmazon()
		{
			//allow this page to been seen only after logging in
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return null;

			return View("Nop.Plugin.Misc.AmazonImport.Views.MiscAmazonImport.SearchAmazon", new AmazonImportModel.AmazonKeywordSearchModel());
		}

		[HttpPost, GridAction(EnableCustomBinding = true)]
		public ActionResult SearchAmazon(GridCommand command, AmazonImportModel.AmazonKeywordSearchModel model)
		{
			
			//allow this page to been seen only after logging in
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return null;

			//create gridModel for returning data
			var gridModel = new GridModel();

			//search on amazon for keywords and display results
			if (model.Keywords != null)
			{
				AmazonSearchResults results = AmazonSearcher.Search(model.Keywords, command.PageSize, command.Page - 1);

				//display the results on a table
				gridModel = new GridModel();
				gridModel.Data = results.Items.Select(x =>
				{
					AmazonImportModel.AmazonSearchResultModel resultModel = new AmazonImportModel.AmazonSearchResultModel();
					resultModel.ASIN = x.ASIN;
					resultModel.Description = x.Description;
					resultModel.DetailPageURL = x.DetailPageURL;
					resultModel.ImageUrl = x.ImageUrl;
					resultModel.Price = x.Price;
					resultModel.EditorialReviews = x.Reviews;
					resultModel.Title = x.Title;
					return resultModel;
				}
				);
				gridModel.Total = System.Math.Min(50, results.TotalItems);
			}
			else //model.keywords == null
			{
				gridModel.Total = 0;
			}

			return new JsonResult
			{
				Data = gridModel
			};
		}

		public ActionResult CreateFromASIN(string ASIN)
		{
			//allow this page to been seen only after logging in
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return null;

			if (ASIN == null) //no ASIN supplied, show form 
			{
				var model = new AmazonImportModel.CreateFromASINModel();
				return View("Nop.Plugin.Misc.AmazonImport.Views.MiscAmazonImport.CreateFromASIN", model);
			}

			//if product already exists, show error
			if(GetAllAmazonProductASINs().Contains(ASIN))
			{
				return View("Nop.Plugin.Misc.AmazonImport.Views.MiscAmazonImport.ItemAlreadyExists");
			}

			//search for product form amazon
			AmazonItem item = AmazonSearcher.GetItem(ASIN);
			if (item != null) //add product to database and open the product editing form
			{
				int productId = AddAmazonItem(item);

				//open "Edit product" page for the newly created product
				return RedirectToAction("Edit", "Product", new { id = productId });
			}
			else //product could no longer be found on amazon
			{
				_logger.Error("Adding product failed: product could no longer be found on Amazon");
				return null;
			}
		}

		[HttpPost]
		public ActionResult CreateFromASIN(AmazonImportModel.CreateFromASINModel model)
		{
			//allow this page to been seen only after logging in
			if (!_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
				return null;

			if (ModelState.IsValid)
			{
				var allASINs = GetAllAmazonProductASINs();
				if (GetAllAmazonProductASINs().Contains(model.ASIN)) //product already exists, show error
				{
					ModelState["ASIN"].Errors.Add("A product with this ASIN already exists");
				}
				else
				{
					AmazonItem item = AmazonSearcher.GetItem(model.ASIN);

					if (item != null)//item was found on Amazon
					{
						//add the created Product to the database
						int productId = AddAmazonItem(item);

						//open "Edit product" page for the newly created product
						return RedirectToAction("Edit", "Product", new { id = productId });
					}
					else //item was not found
					{
						ModelState["ASIN"].Errors.Add("An item with this ASIN was not found on Amazon");
					}
				}//if (GetAllAmazonProductASINs().Contains(model.ASIN)) 
			} //if ModelState.IsValid

			//problem occured, display the form again with an appropriate error message
			return View("Nop.Plugin.Misc.AmazonImport.Views.MiscAmazonImport.CreateFromASIN", model);
		}

	}
}