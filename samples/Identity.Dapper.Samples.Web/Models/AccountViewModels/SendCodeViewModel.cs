using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Dapper.Samples.Web.Models.AccountViewModels
{
    public class SendCodeViewModel
    {
        public SendCodeViewModel()
        {
            Providers = new List<SelectListItem>();
        }

        public string? SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; private set; }

#pragma warning disable CA1056 // Uri properties should not be strings
        public string? ReturnUrl { get; set; }
#pragma warning restore CA1056 // Uri properties should not be strings

        public bool RememberMe { get; set; }
    }
}
