using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Identity.Dapper.Samples.Web.Models.ManageViewModels
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public IList<UserLoginInfo>? Logins { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public string? PhoneNumber { get; set; }

        public bool TwoFactor { get; set; }

        public bool BrowserRemembered { get; set; }
    }
}
