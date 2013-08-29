using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

using System.ServiceModel;
using Nop.Plugin.Misc.AmazonImport.AmazonApiServiceReference;

// Author Tanel Sirp

namespace Nop.Plugin.Misc.AmazonImport.AmazonHelper
{
	public class AmazonSearcher
	{
		static string[] _RESPONSE_GROUPS = new string[]{ /*"ItemAttributes",*/ "Images", "EditorialReview", "Small", "OfferSummary", "Large" };
		const string _SECRET_KEY = "EkMCWFcbXWIbMn9inPa87p+VJBwaq7U3XEvWMJqI";
		const string _ACCESS_KEY_ID = "AKIAJ24GA7TRBF47QK7A";
		const string _ASSOCIATE_TAG = "fake0d1-20";


		public static AmazonItem GetItem(string ASIN)
		{
			AmazonItem foundItem = null;

			// make an array with only one string and use it to find items on amazon
			Item[] returnedItems = DoAmazonItemLookup(new string[] { ASIN });

			if (returnedItems == null)
			{
				// request did not return any items, a product with this ASIN doesnt exist
				return null;
			}

			//create AmazonItem object from returned Item object
			foundItem = new AmazonItem(returnedItems[0]);

			return foundItem;
		}

		public static AmazonItem[] GetItems(string[] ASINs)
		{
			AmazonItem[] items = new AmazonItem[ASINs.Length];

			//max 10 items allowed per request, do multiple requests
			for (int i = 0; i < ASINs.Length; i += 10)
			{
				// create an array with 10 ASINs
				string[] ASINArray = new string[10];
				for (int j = i; j < i + 10 && j < ASINs.Length; j++)
				{
					ASINArray[j] = ASINs[i + j];
				}

				// get response from amazon api
				Item[] returnedItems = DoAmazonItemLookup(ASINArray);

				// create AmazonItem objects from Item objects and add them to the array
				// if there is a problem with any items, or if there are duplicates, amazon will return less items. so if ASINs do not match, skip ASIN (increase skipped)
				int skipped = 0;
				for(int k = 0; k < returnedItems.Length; k++)
				{
					while(ASINs[i + k + skipped] != returnedItems[k].ASIN)
						skipped++;

					items[i + k + skipped] = new AmazonItem(returnedItems[k]);
				}
			}

			return items;
		}

		public static AmazonSearchResults Search(string keywords, int resultsRequested, int page)
		{
			AmazonSearchResults results = new AmazonSearchResults();
			results.Items = new List<AmazonItem>();

			List<Item> items = new List<Item>();
			
			int itemsCount = 0;//results added to list
			int itemIndex = page * resultsRequested;//item index among all results found by amazon
			int totalResults = 0;//results found by amazon

			bool shouldFinish = false;
			while (itemsCount < resultsRequested && !shouldFinish)
			{
				ItemSearchResponse response = DoAmazonItemSearch(keywords, (itemIndex / 10) + 1);

				if (response == null)
				{
					Debug.WriteLine("Response was null");
					break;
				}
				if (response.Items[0].Item == null)
				{
					Debug.WriteLine("No items returned");
					break;
				}
				else
				{
					totalResults = Convert.ToInt32(response.Items[0].TotalResults);
				}

				for(int i = itemIndex % 10; i < 10; i++)
				{
					if (itemIndex >= totalResults || itemIndex >= 50 || itemsCount >= resultsRequested)
					{
						shouldFinish = true;
						break;
					}

					Item item = response.Items[0].Item[i];
					results.Items.Add(new AmazonItem(item));
					itemsCount++;
					itemIndex++;
				}
			}

			results.TotalItems = totalResults;
			return results;
		}

		


		private static ItemSearchResponse DoAmazonItemSearch(string keywords, int page)//AWSECommerceServicePortTypeClient ecs, ItemSearchRequest request)
		{
			ItemSearchResponse response = null;

			// Create an ItemSearch wrapper
			ItemSearch search = new ItemSearch();
			search.AssociateTag = _ASSOCIATE_TAG;
			search.AWSAccessKeyId = _ACCESS_KEY_ID;

			AWSECommerceServicePortTypeClient ecs = GetClient();
			ItemSearchRequest request = GetSearchRequest();
			request.Keywords = keywords;
			request.ItemPage = Convert.ToString(page);

			// Set the request on the search wrapper
			search.Request = new ItemSearchRequest[] { request };

			try
			{
				//Send the request and store the response
				response = ecs.ItemSearch(search);
			}
			catch (NullReferenceException e)
			{
				Debug.WriteLine(e.ToString());
			}
			return response;
		}

		private static Item[] DoAmazonItemLookup(string[] ASINs)
		{
			ItemLookupResponse response = null;

			AWSECommerceServicePortTypeClient ecs = GetClient();
			ItemLookupRequest request = GetLookupRequest();
			request.ItemId = ASINs;

			ItemLookup lookup = new ItemLookup();
			lookup.AssociateTag = _ASSOCIATE_TAG;
			lookup.AWSAccessKeyId = _ACCESS_KEY_ID;
			
			// Set the request on the search wrapper
			lookup.Request = new ItemLookupRequest[] { request };

			try
			{
				//Send the request and store the response
				response = ecs.ItemLookup(lookup);
			}
			catch (NullReferenceException e)
			{
				Debug.WriteLine(e.ToString());
			}

			if (response == null)
			{
				throw new Exception("Request did not return a response. Error using Amazon API");
			}

			return response.Items[0].Item;
		}



		private static AWSECommerceServicePortTypeClient GetClient()
		{
			// Create an instance of the Product Advertising API service
			BasicHttpBinding binding = new BasicHttpBinding(BasicHttpSecurityMode.Transport);
			binding.MaxReceivedMessageSize = int.MaxValue;

			AWSECommerceServicePortTypeClient ecs = new AWSECommerceServicePortTypeClient(
						binding,
						new EndpointAddress("https://webservices.amazon.com/onca/soap?Service=AWSECommerceService"));

			// add authentication to the ECS client
			ecs.ChannelFactory.Endpoint.Behaviors.Add(new AmazonSigningEndpointBehavior(_ACCESS_KEY_ID, _SECRET_KEY));

			return ecs;
		}

		private static ItemSearchRequest GetSearchRequest()
		{
			// Create a request object
			ItemSearchRequest request = new ItemSearchRequest();
			// Fill the request object with request parameters
			request.ResponseGroup = _RESPONSE_GROUPS;

			// Set SearchIndex and Keywords
			request.SearchIndex = "All";

			return request;
		}

		private static ItemLookupRequest GetLookupRequest()
		{
			ItemLookupRequest request = new ItemLookupRequest();
			request.ResponseGroup = _RESPONSE_GROUPS;
			return request;
		}

	}
}