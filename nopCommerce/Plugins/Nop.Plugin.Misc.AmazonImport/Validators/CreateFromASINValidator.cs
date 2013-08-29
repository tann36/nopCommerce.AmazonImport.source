using Nop.Services.Localization;
using System;
using FluentValidation;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Misc.AmazonImport.Validators
{
	public class CreateFromASINValidator : AbstractValidator<Nop.Plugin.Misc.AmazonImport.Models.AmazonImportModel.CreateFromASINModel>
    {
		public CreateFromASINValidator(ILocalizationService localizationService)
		{
			RuleFor(x => x.ASIN)
			.NotEmpty()
			//.WithMessage("An ASIN is required");
			.WithMessage(localizationService.GetResource("Plugins.Misc.AmazonImport.ASINRequired"));
        }
    }
}
