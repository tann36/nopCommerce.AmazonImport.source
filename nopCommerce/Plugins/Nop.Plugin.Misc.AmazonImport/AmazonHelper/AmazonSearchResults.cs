using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Misc.AmazonImport.AmazonHelper
{
	public class AmazonSearchResults
	{
		public int TotalItems { get; set; }
		public List<AmazonItem> Items { get; set; }
	}
}