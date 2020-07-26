using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Identity.Dapper.Samples.Web.Models.ManageViewModels
{
    public class ConfigureTwoFactorViewModel
    {
        public ConfigureTwoFactorViewModel()
        {
            Providers = new List<SelectListItem>();
        }

        public string? SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; private set; }
    }
}
